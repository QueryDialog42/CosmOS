using System.IO;
using System.Windows;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Controls;

namespace WPFFrameworkApp
{
    /// <summary>
    /// PNGWindow.xaml etkileşim mantığı
    /// </summary>
    public partial class PicWindow : Window
    {
        public MainWindow window;
        public string desktopPath;
        private SolidColorBrush menucolor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(RoutineLogics.GetColorSettingsFromCcol()[3]));
        private string filter = $"PNG files (*{SupportedFiles.PNG})|*{SupportedFiles.PNG}|JPG files (*{SupportedFiles.JPG})|*{SupportedFiles.JPG}";

        public PicWindow()
        {
            InitializeComponent();
            SetStyles();
            Show();
        }

        private void SetStyles()
        {
            RoutineLogics.SetSettingsForAllMenu(PictureMenu, RoutineLogics.GetFontSettingsFromCfont());
            MenuItem[] items = { PItem1, PItem2, PItem3, PItem4, PItem5, PItem6}; 

            foreach (MenuItem item in items) item.Background = menucolor;
        }

        private void PictureAdd_Wanted(object sender, RoutedEventArgs e)
        {
            RoutineLogics.MoveAnythingWithQuery("Add Picture", filter, null, desktopPath, desktopPath, 4);
            RoutineLogics.ReloadDesktop(window, desktopPath);
        }

        private void PictureMove_Wanted(object sender, RoutedEventArgs e)
        {
            RoutineLogics.MoveAnythingWithQuery("Move Picture", filter, Title, desktopPath, desktopPath, 1);
            RoutineLogics.ReloadDesktop(window, desktopPath);
            Close();
        }

        private void PictureCopy_Wanted(object sender, RoutedEventArgs e)
        {
            RoutineLogics.CopyAnythingWithQuery("Copy Picture", filter, Title, desktopPath, desktopPath);
            RoutineLogics.ReloadDesktop(window, desktopPath);
            Close();
        }

        private void PictureRename_Wanted(object sender, RoutedEventArgs e)
        {
            string fileimage;

            if (Title.EndsWith(SupportedFiles.PNG)) fileimage = ImagePaths.PNG_IMG;
            else if (Title.EndsWith(SupportedFiles.JPG)) fileimage = ImagePaths.JPG_IMG;
            else fileimage = ImagePaths.RENM_IMG;

            RoutineLogics.RenameFile_Wanted(Path.Combine(desktopPath, Title), fileimage);
            RoutineLogics.ReloadDesktop(window, desktopPath);
            Close();
        }

        private void PictureDelete_Wanted(object sender, RoutedEventArgs e)
        {
            string[] options = { "Ok", "Cancel" };
            if (MessageBox.Show($"Are you sure you want to delete {Title}", "Delete Picture", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                RoutineLogics.MoveAnythingWithoutQuery(desktopPath, Title, MainWindow.TrashPath);
                RoutineLogics.ReloadDesktop(window, desktopPath);
                Close();
            }
        }
    }
}
