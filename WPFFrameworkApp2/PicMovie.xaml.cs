using System;
using System.IO;
using System.Windows;
using WPFFrameworkApp2;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.Generic;

namespace WPFFrameworkApp
{
    /// <summary>
    /// PicMovie.xaml etkileşim mantığı
    /// </summary>
    public partial class PicMovie : Window, IWindow
    {
        public MainWindow window;
        public string desktopPath;
        private string picVideoFilter = $"PNG files (*{SupportedFiles.PNG})|*{SupportedFiles.PNG}|JPG files (*{SupportedFiles.JPG})|*{SupportedFiles.JPG}|MP4 files (*{SupportedFiles.MP4})|*{SupportedFiles.MP4}";
        private string[] fontsettings = RoutineLogics.GetFontSettingsFromCfont();
        public PicMovie()
        {
            InitializeComponent();
            ReloadWindow();
            setStyles();
            Show();
        }

        #region Reload functions
        public void ReloadWindow()
        {
            picMovieDesktop.Children.Clear();

            IEnumerable<string> picvideos = Directory.EnumerateFileSystemEntries(MainWindow.PicVideoPath);

            foreach (string picvideo in picvideos)
            {
                if (picvideo.EndsWith(SupportedFiles.PNG) || picvideo.EndsWith(SupportedFiles.JPG)) InitPictureForPicMovie(picvideo);
                else if (picvideo.EndsWith(SupportedFiles.MP4)) InitVideoForPicMovie(picvideo);
            }
        }
        #endregion

        #region MenuStyles functions
        private void setStyles()
        {
            MenuItem[] items = { PVItem1, PVItem2, PVItem3, PVItem4, PVItem5, PVItem6, PVItem7 };
            RoutineLogics.SetWindowStyles(picVideoMenu, items);
        }
        #endregion

        #region App Creation functions 
        private void InitPictureForPicMovie(string filepath)
        {
            string filename = Path.GetFileName(filepath);

            Button app = RoutineLogics.CreateButton(filename, 20);
            TextBlock appname = RoutineLogics.CreateTextBlock(filename, 0);
            Image image = RoutineLogics.CreateImage(20);
            StackPanel stackpanel = new StackPanel
            {
                Orientation = Orientation.Vertical
            };
            RoutineLogics.Appearence(image, stackpanel, app, appname, filepath);

            picMovieDesktop.Children.Add(app);

            app.Click += (s, e) =>
            {
                PicWindow pictureApp = RoutineLogics.OpenPicWindow(window, window.currentDesktop, filename);

                pictureApp.PicMain.Source = RoutineLogics.setBitmapImage(filepath);
                pictureApp.isInPicFolder = true;
                pictureApp.picmovie = this;
                pictureApp.SizeToContent = SizeToContent.WidthAndHeight;
            };
        }
        private void InitVideoForPicMovie(string filepath)
        {
            string filename = Path.GetFileName(filepath);

            Button app = RoutineLogics.CreateButton(filename, 20);
            TextBlock appname = RoutineLogics.CreateTextBlock(filename, 0);
            Image image = RoutineLogics.CreateImage(20);
            StackPanel stackpanel = new StackPanel
            {
                Orientation = Orientation.Vertical
            };
            RoutineLogics.Appearence(image, stackpanel, app, appname, ImagePaths.MP4_IMG);

            picMovieDesktop.Children.Add(app);

            app.Click += (sender, e) =>
            {
                var videoWindow = new VideoWindow 
                {
                    Title = filename,
                    desktopPath = MainWindow.PicVideoPath
                };
                videoWindow.videoPlayer.Source = new Uri(filepath);
            };
        }
        #endregion

        #region PicMovie menuitem options functions
        private void AddPicVideo_Wanted(object sender, RoutedEventArgs e)
        {
            RoutineLogics.MoveAnythingWithQuery("Add Picture/Video", picVideoFilter, null, desktopPath, desktopPath, 5);
            RoutineLogics.ReloadWindow(window);
            ReloadWindow();
        }
        private void MovePicVideo_Wanted(object sender, RoutedEventArgs e)
        {
            RoutineLogics.MoveAnythingWithQuery("Move Picture/Video", picVideoFilter, null, MainWindow.PicVideoPath, MainWindow.PicVideoPath, 1);
            RoutineLogics.ReloadWindow(window);
            ReloadWindow(); 
        }
        private void CopyPicVideo_Wanted(object sender, RoutedEventArgs e)
        {
            RoutineLogics.CopyAnythingWithQuery("Copy Picture/Video", picVideoFilter, null, MainWindow.PicVideoPath, MainWindow.PicVideoPath);
            RoutineLogics.ReloadWindow(window);
            ReloadWindow();
        }
        private void DeletePicVideo_Wanted(object sender, RoutedEventArgs e)
        {
            RoutineLogics.MoveAnythingWithQuery("Delete PicVideo", picVideoFilter, null, MainWindow.PicVideoPath, desktopPath, 3);
            RoutineLogics.ReloadWindow(window);
            ReloadWindow();
        }
        private void ReloadWindow_Wanted(object sender, RoutedEventArgs e)
        {
            ReloadWindow();
        }
        private void AboutPage_Wanted(object sender, RoutedEventArgs e)
        {
            RoutineLogics.ShowAboutWindow(AppTitles.APP_PICMOV, ImagePaths.PVAPP_IMG, ImagePaths.PVAPP_IMG, Versions.PICMOV_VRS, Messages.ABT_DFLT_MSG);
        }
        #endregion
    }
}
