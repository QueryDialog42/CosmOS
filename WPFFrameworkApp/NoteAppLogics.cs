using Microsoft.Win32;
using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace WPFFrameworkApp
{
    public static class NoteAppLogics
    {
        #region NoteApp menuitems functions
        public static void NewNote_Wanted(MainWindow windowForNote, string currentDesktopForNote)
        {
            string[] options = { "TXT", "RTF" };
            switch (QueryDialog.ShowQueryDialog("Choose Note Type", "NoteApp", options, ImagePaths.NADD_IMG, ImagePaths.NOTE_IMG))
            {
                case 0: CreateNoteAppEnvironment(windowForNote, currentDesktopForNote, new TXTNote(), true); break;
                case 1: CreateNoteAppEnvironment(windowForNote, currentDesktopForNote, new RTFNote(), false); break;
            }
        }
        public static void OpenNote_Wanted(MainWindow windowForNote, string currentDesktopForNote)
        {
            OpenFileDialog fileDialog = new OpenFileDialog
            {
                InitialDirectory = currentDesktopForNote,
                Filter = $"Text Files (*{SupportedFiles.TXT})|*{SupportedFiles.TXT}|RTF Files (*{SupportedFiles.RTF})|*{SupportedFiles.RTF}", // TXT files and RTF files are supported
                Title = "Open Note"
            };

            if (fileDialog.ShowDialog() == true)
            {
                string file = fileDialog.FileName;
                string folder = Path.GetDirectoryName(file);
                if (file.EndsWith(SupportedFiles.TXT)) OpenTXTFile(windowForNote, folder, file);
                else if (file.EndsWith(SupportedFiles.RTF)) OpenRTFFile(windowForNote, folder, file);
                else RoutineLogics.ErrorMessage(Errors.UNSUPP_ERR, file, " is not supported for ", Versions.GOS_VRS, "\n.txt\n.rtf\nis supported for now.");
            }
        }
        public static void TXTSaveNote_Wanted(string currentDesktopForNote, TXTNote noteapp)
        {
            string filename = noteapp.Title;
            try
            {
                using (StreamWriter writer = new StreamWriter(Path.Combine(currentDesktopForNote, filename)))
                {
                    writer.WriteLine(noteapp.note.Text);
                }
            }
            catch (Exception ex)
            {
                RoutineLogics.ErrorMessage("TXT " + Errors.SAVE_ERR, Errors.SAVE_ERR_MSG, filename, "\n", ex.Message);
            }
        }
        public static void TXTSaveAsNote_Wanted(MainWindow windowForNote, string currentDesktopForNote, TXTNote noteapp)
        {
            SaveFileDialog filedialog = new SaveFileDialog
            {
                InitialDirectory = currentDesktopForNote,
                Filter = $"Text Files ({SupportedFiles.TXT})|*{SupportedFiles.TXT}",
                Title = "Save As"
            };
            if (filedialog.ShowDialog() == true)
            {
                string file = filedialog.FileName;
                try
                {
                    using (StreamWriter writer = new StreamWriter(file))
                    {
                        writer.WriteLine(noteapp.note.Text);
                    }
                    RoutineLogics.ReloadDesktop(windowForNote, currentDesktopForNote);
                    noteapp.Close();
                }
                catch (Exception ex)
                {
                    RoutineLogics.ErrorMessage("TXT " + Errors.SAVE_ERR, Errors.SAVE_ERR_MSG, Path.GetFileName(file), "\n", ex.Message);
                }
            }
        }
        public static void RTFSaveNote_Wanted(string currentDesktopForNote, RTFNote noteapp)
        {
            string filename = noteapp.Title;
            try
            {
                TextRange textrange = new TextRange(noteapp.RichNote.Document.ContentStart, noteapp.RichNote.Document.ContentEnd);
                using (FileStream filestream = new FileStream(Path.Combine(currentDesktopForNote, filename), FileMode.Create))
                {
                    textrange.Save(filestream, DataFormats.Rtf);
                }
            }
            catch (Exception ex)
            {
                RoutineLogics.ErrorMessage("RTF " + Errors.SAVE_ERR, Errors.SAVE_ERR_MSG, filename, "\n", ex.Message);
            }
        }
        public static void RTFSaveAsNote_Wanted(MainWindow windowForNote, string currentDesktopForNote, RTFNote noteapp)
        {
            SaveFileDialog filedialog = new SaveFileDialog
            {
                InitialDirectory = currentDesktopForNote,
                Filter = $"RTF Files (*{SupportedFiles.RTF})|*{SupportedFiles.RTF}",
                Title = "Save As"
            };
            if (filedialog.ShowDialog() == true)
            {
                string file = filedialog.FileName;
                try
                {
                    TextRange textrange = new TextRange(noteapp.RichNote.Document.ContentStart, noteapp.RichNote.Document.ContentEnd);
                    using (FileStream filestream = new FileStream(file, FileMode.Create))
                    {
                        textrange.Save(filestream, DataFormats.Rtf);
                    }
                    RoutineLogics.ReloadDesktop(windowForNote, currentDesktopForNote);
                    noteapp.Close();
                }
                catch (Exception ex)
                {
                    RoutineLogics.ErrorMessage("RTF " + Errors.SAVE_ERR, Errors.SAVE_ERR_MSG, Path.GetFileName(file), "\n", ex.Message);
                }
            }
        }
        public static void CopyNote_Wanted(MainWindow windowForNote, string currentDesktopForNote, string filter, dynamic noteapp)
        {
            RoutineLogics.CopyAnythingWithQuery("Copy Note", filter, noteapp.Title, currentDesktopForNote, currentDesktopForNote);
            noteapp.Close();
            RoutineLogics.ReloadDesktop(windowForNote, currentDesktopForNote);

        }
        public static void MoveNote_Wanted(MainWindow windowForNote, string currentDesktopForNote, string filter, dynamic noteapp)
        {
            RoutineLogics.MoveAnythingWithQuery("Move Note", filter, noteapp.Title, currentDesktopForNote, currentDesktopForNote, 1);
            noteapp.Close();
            RoutineLogics.ReloadDesktop(windowForNote, currentDesktopForNote);

        }
        public static void DeleteNote_Wanted(MainWindow windowForNote, string currentDesktopForNote, dynamic noteapp)
        {
            string filename = noteapp.Title;
            if (MessageBox.Show($"Are you sure you want to delete {filename}?", "Delete Note", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {

                RoutineLogics.MoveAnythingWithoutQuery(currentDesktopForNote, filename, Path.Combine(MainWindow.TrashPath, filename));
                noteapp.Close();
                RoutineLogics.ReloadDesktop(windowForNote, currentDesktopForNote);
            }
        }
        #endregion

        #region NoteApp open function
        private static void OpenTXTFile(MainWindow windowForNote, string folder, string filepath)
        {
            TXTNote TXTnoteapp = new TXTNote
            {
                currentDesktopForNote = folder,
                windowForNote = windowForNote // something different needed
            };
            try
            {
                using (StreamReader reader = new StreamReader(filepath))
                {
                    StringBuilder stringbuilder = new StringBuilder();
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        stringbuilder.Append(line);
                    }
                    TXTnoteapp.note.Text = stringbuilder.ToString();
                }
                TXTnoteapp.Title = Path.GetFileName(filepath);
            }
            catch (Exception ex)
            {
                RoutineLogics.ErrorMessage(Errors.READ_ERR, Errors.READ_ERR_MSG, Path.GetFileName(filepath), "\n", ex.Message);
            }
        }
        private static void OpenRTFFile(MainWindow windowForNote, string folder, string filepath)
        {
            RTFNote RTFnoteapp = new RTFNote
            {
                currentDesktopForNote = folder,
                windowForNote = windowForNote
            };
            try
            {
                using (StreamReader reader = new StreamReader(filepath))
                {
                    StringBuilder stringbuilder = new StringBuilder();
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        stringbuilder.Append(line);
                    }

                    RTFnoteapp.Title = Path.GetFileName(filepath);
                    // Read the RTF file content
                    string rtfContent = File.ReadAllText(filepath);

                    // Create a TextRange to load the RTF content into the RichTextBox
                    TextRange textRange = new TextRange(RTFnoteapp.RichNote.Document.ContentStart, RTFnoteapp.RichNote.Document.ContentEnd);

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        // Write the RTF content to the memory stream
                        using (StreamWriter writer = new StreamWriter(memoryStream))
                        {
                            writer.Write(rtfContent);
                            writer.Flush();
                            memoryStream.Position = 0; // Reset the stream position

                            // Load the RTF content into the RichTextBox
                            textRange.Load(memoryStream, DataFormats.Rtf);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                RoutineLogics.ErrorMessage(Errors.READ_ERR, Errors.READ_ERR_MSG, Path.GetFileName(filepath), "\n", ex.Message);
            }
        }
        #endregion

        #region RTF style wanted functions
        public static void FontChange_Wanted(RichTextBox RichNote, double value)
        {
            TextSelection selection = RichNote.Selection;
            if (selection.IsEmpty == false)
            {
                TextRange textrange = new TextRange(selection.Start, selection.End);

                // Get the current font size
                var currentFontSize = textrange.GetPropertyValue(TextElement.FontSizeProperty);

                // Check if the current font size is a valid double
                if (currentFontSize is double currentSize)
                {
                    // Apply the new font size to the selected text
                    textrange.ApplyPropertyValue(TextElement.FontSizeProperty, currentSize + value);
                }
                else
                {
                    // Apply the new font size to the selected text
                    textrange.ApplyPropertyValue(TextElement.FontSizeProperty, 12.0);
                }
            }
        }
        public static void ColorChange_Wanted(RichTextBox RichNote)
        {
            System.Windows.Forms.ColorDialog colordialog = new System.Windows.Forms.ColorDialog();
            if (colordialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                System.Drawing.Color selectedcolor = colordialog.Color;

                // Convert to WPF Color
                Color wpfcolor = Color.FromArgb(selectedcolor.A, selectedcolor.R, selectedcolor.G, selectedcolor.B);

                TextSelection selection = RichNote.Selection;
                if (selection.IsEmpty == false)
                {
                    TextRange textRange = new TextRange(selection.Start, selection.End);

                    // Apply the new foreground color to the selected text
                    textRange.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(wpfcolor));
                }
            }
        }
        #endregion

        #region Unclassified public functions
        public static void CreateNoteAppEnvironment(MainWindow windowForNote, string currentDesktopForNote, dynamic noteapp, bool isButtonNull)
        {
            noteapp.currentDesktopForNote = currentDesktopForNote;
            noteapp.windowForNote = windowForNote;
            noteapp.save.IsEnabled = false;
            noteapp.copy.IsEnabled = false;
            noteapp.move.IsEnabled = false;
            noteapp.rename.IsEnabled = false;
            noteapp.delete.IsEnabled = false;
            if (isButtonNull == false) noteapp.saveButton.IsEnabled = false;
        }
        #endregion
    }
}