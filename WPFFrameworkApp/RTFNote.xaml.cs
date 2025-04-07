using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Forms;
using System.ComponentModel;
using System.Windows.Documents;

namespace WPFFrameworkApp
{
    /// <summary>
    /// RTFNote.xaml etkileşim mantığı
    /// </summary>
    public partial class RTFNote : Window
    {
        public string currentDesktopForNote;
        public MainWindow windowForNote;
        private string filter = $"RTF Files(*{SupportedFiles.RTF}) | *{SupportedFiles.RTF}";

        public RTFNote()
        {
            InitializeComponent();
            RoutineLogics.SetSettingsForAllMenu(fileMenu, RoutineLogics.GetFontSettingsFromCfont());
            PaintAllMenuItem();
            Show();
        }

        #region RTFnote menuitems functions
        private void NewNote(object sender, RoutedEventArgs e)
        {
            NoteAppLogics.NewNote_Wanted(windowForNote, currentDesktopForNote);
        }
        private void OpenNote(object sender, RoutedEventArgs e)
        {
            NoteAppLogics.OpenNote_Wanted(windowForNote, currentDesktopForNote);
        }
        private void SaveNote(object sender, RoutedEventArgs e)
        {
            NoteAppLogics.RTFSaveNote_Wanted(currentDesktopForNote, this);
        }
        private void SaveAsNote(object sender, RoutedEventArgs e)
        {
            NoteAppLogics.RTFSaveAsNote_Wanted(windowForNote, currentDesktopForNote, this);
        }
        private void CopyNote(object sender, RoutedEventArgs e)
        {
            NoteAppLogics.CopyNote_Wanted(windowForNote, currentDesktopForNote, filter, this);
        }
        private void MoveNote(object sender, RoutedEventArgs e)
        {
            NoteAppLogics.MoveNote_Wanted(windowForNote, currentDesktopForNote, filter, this);
        }
        private void RenameNote(object sender, RoutedEventArgs e)
        {
            RoutineLogics.RenameFile_Wanted(Path.Combine(currentDesktopForNote, Title), ImagePaths.NADD_IMG, ImagePaths.NADD_IMG);
            RoutineLogics.ReloadDesktop(windowForNote, currentDesktopForNote);
            Close();
        }
        private void DeleteNote(object sender, RoutedEventArgs e)
        {
            NoteAppLogics.DeleteNote_Wanted(windowForNote, currentDesktopForNote, this);
        }
        private void AboutGennotePage_Wanted(object sender, RoutedEventArgs e)
        {
            RoutineLogics.ShowAboutWindow("GenNote", ImagePaths.NOTE_IMG, ImagePaths.NOTE_IMG, Versions.NOTE_VRS, Messages.ABT_DFLT_MSG);
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

        #region OnClosing functions
        protected override void OnClosing(CancelEventArgs e)
        {
            currentDesktopForNote = null;
            windowForNote = null;
            filter = null;
        }
        #endregion

        #region Unclassified private functions
        private void PaintAllMenuItem()
        {
            string[] colors = RoutineLogics.GetColorSettingsFromCcol();
            string color = colors[3]; // menu background color
            System.Windows.Controls.MenuItem[] items = { filemenu, openmenu, save, saveasmenu, copy, move, rename, delete, aboutmenu };
            foreach (System.Windows.Controls.MenuItem item in items)
            {
                PaintMenuItem(item, color);
            }
        }
        private void PaintMenuItem(System.Windows.Controls.MenuItem menuitem, string color)
        {
            menuitem.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
            menuitem.BorderThickness = new Thickness(0);
        }
        #endregion
    }
}
