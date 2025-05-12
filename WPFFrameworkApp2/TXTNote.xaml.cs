using System.IO;
using System.Windows;
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
            MenuItem[] items = { filemenu, openmenu, save, saveasmenu, copy, move, rename, delete, aboutmenu };
            RoutineLogics.SetWindowStyles(fileMenubar, items);
        }
        #endregion

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
            NoteAppLogics.TXTSaveNote_Wanted(this);
        }
        public void SaveAsFile(object sender, RoutedEventArgs e)
        {
            NoteAppLogics.TXTSaveAsNote_Wanted(windowForNote, currentDesktopForNote, this);
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
            RoutineLogics.RenameFile_Wanted(Path.Combine(currentDesktopForNote, Title), ImagePaths.TXT_IMG);
            RoutineLogics.ReloadWindow(windowForNote, MainWindow.CDesktopDisplayMode);
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

    }
}
