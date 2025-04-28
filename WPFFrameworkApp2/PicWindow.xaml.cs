using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace WPFFrameworkApp
{
    /// <summary>
    /// PNGWindow.xaml etkileşim mantığı
    /// </summary>
    public partial class PicWindow : Window
    {
        public MainWindow window;
        public PicMovie picmovie;
        public string desktopPath;
        public bool isInPicFolder;
        private string filter = $"PNG files (*{SupportedFiles.PNG})|*{SupportedFiles.PNG}|JPG files (*{SupportedFiles.JPG})|*{SupportedFiles.JPG}";

        public PicWindow()
        {
            InitializeComponent();
            SetStyles();
            Show();
        }

        #region Panel style functions
        private void SetStyles()
        {
            MenuItem[] items = { PItem1, PItem2, PItem3, PItem4, PItem5, PItem6 };
            RoutineLogics.SetWindowStyles(PictureMenu, items);
        }
        #endregion

        #region Picture menuitem options functions
        private void PictureAdd_Wanted(object sender, RoutedEventArgs e)
        {
            switchOfPicMovieLogics(1);
        }
        private void PictureMove_Wanted(object sender, RoutedEventArgs e)
        {
            switchOfPicMovieLogics(2);
        }
        private void PictureCopy_Wanted(object sender, RoutedEventArgs e)
        {
            switchOfPicMovieLogics(3);
        }
        private void PictureRename_Wanted(object sender, RoutedEventArgs e)
        {
            switchOfPicMovieLogics(4);
        }
        private void PictureDelete_Wanted(object sender, RoutedEventArgs e)
        {
            string[] options = { "Ok", "Cancel" };
            if (MessageBox.Show($"Are you sure you want to delete {Title}", "Delete Picture", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                switchOfPicMovieLogics(5);
            }
        }
        private void inPicVideosDo(byte which)
        {
            switch (which)
            {
                case 1: RoutineLogics.MoveAnythingWithQuery("Add Picture", filter, null, desktopPath, MainWindow.PicVideoPath, 5); break;
                case 2: RoutineLogics.MoveAnythingWithQuery("Move Picture", filter, Title, MainWindow.PicVideoPath, MainWindow.PicVideoPath, 1); break;
                case 3: RoutineLogics.CopyAnythingWithQuery("Copy Picture", filter, Title, MainWindow.PicVideoPath, MainWindow.PicVideoPath); break;
                case 4: RoutineLogics.RenameFile_Wanted(Path.Combine(MainWindow.PicVideoPath, Title), Title.EndsWith(SupportedFiles.PNG) ? ImagePaths.PNG_IMG : ImagePaths.JPG_IMG); break;
                case 5: RoutineLogics.MoveAnythingWithoutQuery(MainWindow.PicVideoPath, Title, Path.Combine(MainWindow.TrashPath, Title)); break;
            }
            RoutineLogics.ReloadWindow(window, MainWindow.CDesktopDisplayMode);
            picmovie?.ReloadWindow();
        }
        private void outOfPicVideosDo(byte which)
        {
            switch (which)
            {
                case 1: RoutineLogics.MoveAnythingWithQuery("Add Picture", filter, null, desktopPath, desktopPath, 4); break;
                case 2: RoutineLogics.MoveAnythingWithQuery("Move Picture", filter, Title, desktopPath, desktopPath, 1); break;
                case 3: RoutineLogics.CopyAnythingWithQuery("Copy Picture", filter, Title, desktopPath, desktopPath); break;
                case 4: RoutineLogics.RenameFile_Wanted(Path.Combine(desktopPath, Title), Title.EndsWith(SupportedFiles.PNG) ? ImagePaths.PNG_IMG : ImagePaths.JPG_IMG); break;
                case 5: RoutineLogics.MoveAnythingWithoutQuery(desktopPath, Title, Path.Combine(MainWindow.TrashPath, Title)); break;
            }
            RoutineLogics.ReloadWindow(window, MainWindow.CDesktopDisplayMode);
        }
        private void switchOfPicMovieLogics(byte which)
        {
            switch (isInPicFolder)
            {
                case true: inPicVideosDo(which); break;
                case false: outOfPicVideosDo(which); break;
            }
            if (which != 1) Close();
        }
        #endregion
    }
}
