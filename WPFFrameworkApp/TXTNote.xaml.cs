using System.IO;
using System.Windows;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Controls;

namespace WPFFrameworkApp
{
    /// <summary>
    /// NoteApp.xaml etkileşim mantığı
    /// </summary>
    public partial class TXTNote : Window, IFile
    {
        public string currentDesktopForNote;
        public MainWindow windowForNote;
        private string filter = $"Text Files(*{SupportedFiles.TXT})|*{SupportedFiles.TXT}";
        public TXTNote()
        {
            InitializeComponent();
            RoutineLogics.SetSettingsForAllMenu(fileMenubar, RoutineLogics.GetFontSettingsFromCfont());
            PaintAllMenuItem();
            Show();
        }

        #region TXTnote menuitems functions
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
            NoteAppLogics.TXTSaveNote_Wanted(currentDesktopForNote, this);
        }
        public void SaveAsFile(object sender, RoutedEventArgs e)
        {
            NoteAppLogics.TXTSaveAsNote_Wanted(windowForNote, currentDesktopForNote, this);
        }
        public void CopyFile(object sender, RoutedEventArgs e)
        {
            NoteAppLogics.TXTSaveNote_Wanted(currentDesktopForNote, this);
        }
        public void MoveFile(object sender, RoutedEventArgs e)
        {
            NoteAppLogics.MoveNote_Wanted(windowForNote, currentDesktopForNote, filter, this);
        }
        public void RenameFile(object sender, RoutedEventArgs e)
        {
            RoutineLogics.RenameFile_Wanted(Path.Combine(currentDesktopForNote, Title), ImagePaths.NADD_IMG);
            RoutineLogics.ReloadWindow(windowForNote, currentDesktopForNote);
            Close();
        }
        public void DeleteFile(object sender, RoutedEventArgs e)
        {
            NoteAppLogics.DeleteNote_Wanted(windowForNote, currentDesktopForNote, this);
        }
        public void AboutPage(object sender, RoutedEventArgs e)
        {
            RoutineLogics.ShowAboutWindow("GenNote", ImagePaths.NOTE_IMG, ImagePaths.NOTE_IMG, Versions.NOTE_VRS, Messages.ABT_DFLT_MSG);
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

        #region MenuStyle functions
        private void PaintAllMenuItem()
        {
            string[] colors = RoutineLogics.GetColorSettingsFromCcol();
            string color = colors[3]; // menu background color
            MenuItem[] items = { filemenu, openmenu, save, saveasmenu, copy, move, rename, delete, aboutmenu };
            foreach(MenuItem item in items)
            {
                PaintMenuItem(item, color);
            }
        }
        private void PaintMenuItem(MenuItem menuitem, string color)
        {
            menuitem.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
            menuitem.BorderThickness = new Thickness(0);
        }
        #endregion

    }
}
