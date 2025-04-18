using System;
using System.IO;
using System.Windows;
using WPFFrameworkApp;
using System.Windows.Input;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Threading;

namespace WPFFrameworkApp2
{
    /// <summary>
    /// VideoWindow.xaml etkileşim mantığı
    /// </summary>
    public partial class VideoWindow : Window, IControlable
    {
        public string desktopPath;
        private double totaltime = 0;
        private short rotateAngle = 90;
        private DispatcherTimer timer;
        private string filter = $"MP4 Files (*{SupportedFiles.MP4})|*{SupportedFiles.MP4}";
        public VideoWindow()
        {
            InitializeComponent();
            StartUp();
            Show();
        }

        #region Slider Functions
        private void SliderPositionChanged(object sender, MouseButtonEventArgs e)
        {
            videoPlayer.Position = TimeSpan.FromSeconds(videoSlider.Value);
            totaltime = videoPlayer.NaturalDuration.TimeSpan.TotalSeconds - videoPlayer.Position.TotalSeconds;
            remainedTime.Text = RoutineLogics.TimeFormat(totaltime);
        }
        private void InitializeSliderLogics()
        {
            if (videoPlayer.NaturalDuration.HasTimeSpan)
            {
                videoSlider.Minimum = 0;
                videoSlider.Maximum = videoPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                totaltime = videoPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                remainedTime.Text = RoutineLogics.TimeFormat(totaltime);

                timer = new DispatcherTimer();

                timer.Tick += UpdateVideoSliderPosition;
                timer.Interval = TimeSpan.FromSeconds(1);
                timer.Start();
            }
        }
        private void UpdateVideoSliderPosition(object sender, EventArgs e)
        {
            videoSlider.Value += 1.035;
            totaltime -= 1.035;
            remainedTime.Text = RoutineLogics.TimeFormat(totaltime);

            if (totaltime <= 0) timer.Stop();
        }
        private void InitializeSliderPosition()
        {
            videoPlayer.Position = TimeSpan.FromSeconds(0);
            remainedTime.Text = RoutineLogics.TimeFormat(totaltime);
            totaltime = videoPlayer.NaturalDuration.TimeSpan.TotalSeconds;
        }
        #endregion

        #region OnClosing function
        protected override void OnClosing(CancelEventArgs e)
        {
            timer?.Stop();
            timer = null;
            videoPlayer.Close();
        }
        #endregion

        #region MouseEvent functions
        private void MouseEntered(object sender, MouseEventArgs e)
        {
            timeControl.Visibility = Visibility.Visible;
            videoPanel1.Visibility = Visibility.Visible;
            videoPanel2.Visibility = Visibility.Visible;
        }
        private void MouseLeaved(object sender, MouseEventArgs e)
        {
            timeControl.Visibility = Visibility.Collapsed;
            videoPanel1.Visibility = Visibility.Collapsed;
            videoPanel2.Visibility = Visibility.Collapsed;
        }
        #endregion

        #region control Panel functions
        public void Play(object sender, RoutedEventArgs e)
        {
            startButton.IsEnabled = false;
            stopButton.IsEnabled = true;
            videoPlayer.Play();
            timer.Start();
        }
        public void Pause(object sender, RoutedEventArgs e)
        {
            startButton.IsEnabled = true;
            stopButton.IsEnabled = false;
            videoPlayer.Pause();
            timer.Stop();
        }
        public void Restart(object sender, RoutedEventArgs e)
        {
            startButton.IsEnabled = false;
            stopButton.IsEnabled = true;
            videoPlayer.Stop();
            videoPlayer.Play();

            videoSlider.Value = 0;
            remainedTime.Text = RoutineLogics.TimeFormat(totaltime);

            InitializeSliderPosition();
            timer.Start();
        }
        public void Back(object sender, RoutedEventArgs e)
        {
            AdjustVideoPosition(-5);
        }
        public void Forward(object sender, RoutedEventArgs e)
        {
            AdjustVideoPosition(5);
        }
        #endregion

        #region MenuItem option functions
        private void RenameVideo(object sender, RoutedEventArgs e)
        {
            Close();
            RoutineLogics.RenameFile_Wanted(Path.Combine(desktopPath, Title), ImagePaths.MP4_IMG);
            RepeatedReload();
        }
        private void MoveVideo(object sender, RoutedEventArgs e)
        {
            Close();
            RoutineLogics.MoveAnythingWithQuery("Move Video", filter, Title, desktopPath, desktopPath, 1);
            RepeatedReload();
        }
        private void CopyVideo(object sender, RoutedEventArgs e)
        {
            Close();
            RoutineLogics.CopyAnythingWithQuery("Copy Video", filter, Title, desktopPath, desktopPath);
        }
        private void DeleteVideo(object sender, RoutedEventArgs e)
        {
            string[] options = { "Ok", "Cancel" };
            if (MessageBox.Show($"Are you sure you want to delete {Title}", "Delete Video", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Close();
                RoutineLogics.MoveAnythingWithoutQuery(desktopPath, Title, Path.Combine(MainWindow.TrashPath, Title));
                RepeatedReload();
            }
        }
        private void RotateVideo(object sender, RoutedEventArgs e)
        {
            // Rotate the video by 90 degrees
            rotateAngle += 90;

            videoPlayer.RenderTransform = new RotateTransform(rotateAngle);
            Grid.SetZIndex(videoPlayer, -1);

            if (rotateAngle >= 360) rotateAngle = 0;
        }
        #endregion

        #region Unclassified private functions
        private void InitializeVideo(object sender, EventArgs e)
        {
            InitializeSliderLogics();
        }
        #endregion

        #region Unclassified private functions
        private void AdjustVideoPosition(int seconds)
        {
            videoPlayer.Position = videoPlayer.Position.Add(TimeSpan.FromSeconds(seconds));
            videoSlider.Value = videoPlayer.Position.TotalSeconds;
            totaltime = videoPlayer.NaturalDuration.TimeSpan.TotalSeconds - videoPlayer.Position.TotalSeconds;
            remainedTime.Text = RoutineLogics.TimeFormat(totaltime);
        }
        private void RepeatedReload()
        {
            RoutineLogics.GetPicMovieWindow()?.ReloadWindow();
            MainWindow window = RoutineLogics.GetMainWindow(desktopPath);
            if (window != null) RoutineLogics.ReloadWindow(window, desktopPath, window.searchComboBox);
        }
        private void StartUp()
        {
            MenuItem[] menuitems = { VItem1, VItem2, VItem3, VItem4, VItem5 };
            RoutineLogics.SetWindowStyles(videoMenu, menuitems);

            videoPlayer.MediaOpened += InitializeVideo;
            videoPlayer.RenderTransform = new RotateTransform(90);

            startButton.IsEnabled = false;
            stopButton.IsEnabled = true;
            videoPlayer.Play();
        }
        #endregion
    }
}
