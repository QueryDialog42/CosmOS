using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Forms;
using System.ComponentModel;
using System.Windows.Documents;
using System.Runtime.Versioning;

namespace WPFFrameworkApp
{
    /// <summary>
    /// RTFNote.xaml etkileşim mantığı
    /// </summary>
    public partial class RTFNote : Window, IFile
    {
        public string currentDesktopForNote;
        public MainWindow windowForNote;
        private string filter = $"RTF Files(*{SupportedFiles.RTF}) | *{SupportedFiles.RTF}";

        public RTFNote()
        {
            InitializeComponent();
            StartUp();
            Show();
        }

        #region OnClosing functions
        protected override void OnClosing(CancelEventArgs e)
        {
            currentDesktopForNote = null;
            windowForNote = null;
            filter = null;
        }
        #endregion

        #region MenuStyle functions
        private void StartUp()
        {
            System.Windows.Controls.MenuItem[] items = { filemenu, openmenu, save, saveasmenu, copy, move, rename, delete, aboutmenu };
            RoutineLogics.SetWindowStyles(RTFfileMenu, items);
        }
        #endregion

        #region Toolbar style functions
        private void MakeBold(object sender, RoutedEventArgs e)
        {
            // Get the current selection
            TextSelection selection = RichNote.Selection;

            // Check if there is any text selected
            if (selection.IsEmpty == false)
            {
                // Create a TextRange from the selection
                TextRange textRange = new TextRange(selection.Start, selection.End);

                // Apply bold formatting
                textRange.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
            }
        }
        private void MakeItalic(object sender, RoutedEventArgs e)
        {
            TextSelection selection = RichNote.Selection;
            if (selection.IsEmpty == false)
            {
                TextRange textrange = new TextRange(selection.Start, selection.End);
                textrange.ApplyPropertyValue(TextElement.FontStyleProperty, FontStyles.Italic);
            }
        }
        private void MakeUnderline(object sender, RoutedEventArgs e)
        {
            TextSelection selection = RichNote.Selection;
            if (selection.IsEmpty == false)
            {
                TextRange textrange = new TextRange(selection.Start, selection.End);
                textrange.ApplyPropertyValue(Inline.TextDecorationsProperty, TextDecorations.Underline);
            }
        }
        private void FontDown(object sender, RoutedEventArgs e)
        {
            NoteAppLogics.FontChange_Wanted(RichNote, -1.0);
        }
        private void FontUp(object sender, RoutedEventArgs e)
        {
            NoteAppLogics.FontChange_Wanted(RichNote, 1.0);
        }
        private void FontChange(object sender, RoutedEventArgs e)
        {
            FontDialog fontdialog = new FontDialog();
            if (fontdialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                System.Drawing.Font selectedfont = fontdialog.Font;
                // Convert to WPF FontFamily
                FontFamily wpfFontFamily = new FontFamily(selectedfont.Name);
                TextSelection selection = RichNote.Selection;

                if (selection.IsEmpty == false)
                {
                    TextRange textRange = new TextRange(selection.Start, selection.End);
                    // Apply the new font family to the selected text
                    textRange.ApplyPropertyValue(TextElement.FontFamilyProperty, wpfFontFamily);
                    textRange.ApplyPropertyValue(TextElement.FontSizeProperty, (double)selectedfont.Size);
                }
            }
        }
        private void ColorChange(object sender, RoutedEventArgs e)
        {
            ColorDialog colordialog = new ColorDialog();
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
        private void RemoveStyles(object sender, RoutedEventArgs e)
        {
            TextSelection selection = RichNote.Selection;
            if (selection.IsEmpty == false)
            {
                TextRange textrange = new TextRange(selection.Start, selection.End);
                textrange.ClearAllProperties();
                textrange.ApplyPropertyValue(TextElement.FontFamilyProperty, new FontFamily(Defaults.FONT));
            }
        }
        #endregion

        #region RTFnote menuitems functions
        public void NewFile(object sender, RoutedEventArgs e)
        {
            NoteAppLogics.NewNote_Wanted(windowForNote, currentDesktopForNote);
        }
        public void OpenFile(object sender, RoutedEventArgs e)
        {
            NoteAppLogics.OpenNote_Wanted(windowForNote, currentDesktopForNote);
        }
        public void SaveFile(object sender, RoutedEventArgs e)
        {
            NoteAppLogics.RTFSaveNote_Wanted(currentDesktopForNote, this);
        }
        public void SaveAsFile(object sender, RoutedEventArgs e)
        {
            NoteAppLogics.RTFSaveAsNote_Wanted(windowForNote, currentDesktopForNote, this);
        }
        public void CopyFile(object sender, RoutedEventArgs e)
        {
            NoteAppLogics.CopyNote_Wanted(windowForNote, currentDesktopForNote, filter, this);
        }
        public void MoveFile(object sender, RoutedEventArgs e)
        {
            NoteAppLogics.MoveNote_Wanted(windowForNote, currentDesktopForNote, filter, this);
        }
        public void RenameFile(object sender, RoutedEventArgs e)
        {
            RoutineLogics.RenameFile_Wanted(Path.Combine(currentDesktopForNote, Title), ImagePaths.RTF_IMG);
            RoutineLogics.ReloadWindow(windowForNote);
            Close();
        }
        public void DeleteFile(object sender, RoutedEventArgs e)
        {
            NoteAppLogics.DeleteNote_Wanted(windowForNote, this);
        }
        public void AboutPage(object sender, RoutedEventArgs e)
        {
            RoutineLogics.ShowAboutWindow("GenNote", ImagePaths.NOTE_IMG, ImagePaths.NOTE_IMG, Versions.NOTE_VRS, Messages.ABT_DFLT_MSG);
        }
        #endregion
    }
}
