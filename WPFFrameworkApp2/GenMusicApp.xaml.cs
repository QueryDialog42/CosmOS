using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Media;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace WPFFrameworkApp
{
    /// <summary>
    /// GenMusicApp.xaml etkileşim mantığı
    /// </summary>
    public partial class GenMusicApp : Window, IWindow, IControlable
    {
        public static string currentAudio;
        public static bool isPaused = true;
        public static DispatcherTimer time;
        public static MediaPlayer mediaPlayer;
        public string musicfilter = $"WAV Files (*{SupportedFiles.WAV})|*{SupportedFiles.WAV}|MP3 Files (*{SupportedFiles.MP3})|*{SupportedFiles.MP3}";

        double totaltime;
        public GenMusicApp()
        {
            InitializeComponent();
            SetMenuStyles();
            ReloadWindow();
            Show();
        }

        #region Slider functions
        public void UpdateSliderPosition(object sender, EventArgs e)
        {
            slider.Value += 1.035; // 1 second front
            totaltime -= 1.035;
            remainedTime.Text = RoutineLogics.TimeFormat(totaltime);
            if (totaltime <= 0)
            {
                time.Stop();
                isPaused = true;
            }
        }
        private void SliderPositionChanged(object sender, MouseButtonEventArgs e) // MouseButtonEventArgs = waits until mouse events, then do action
        {
            mediaPlayer.Position = TimeSpan.FromSeconds(slider.Value);
            totaltime = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds - mediaPlayer.Position.TotalSeconds;
            remainedTime.Text = RoutineLogics.TimeFormat(totaltime);
        }
        private void InitializeSliderLogics()
        {
            if (mediaPlayer.NaturalDuration.HasTimeSpan) // slider starts with -1 minimum value, if minimum value is not -1, then slider is set
            {
                slider.Minimum = 0;
                slider.Maximum = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds; // get the total seconds of media player
                slider.Value = mediaPlayer.Position.TotalSeconds; // get the current second of media player
                totaltime = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds - mediaPlayer.Position.TotalSeconds;
                remainedTime.Text = RoutineLogics.TimeFormat(totaltime); // minutes:seconds
                if (time == null)
                {
                    time = new DispatcherTimer();
                    time.Tick += UpdateSliderPosition;
                    time.Interval = TimeSpan.FromSeconds(1);
                    time.Start();
                }
                else if (!time.IsEnabled)
                {
                    time.Start();
                }
            }
            else // if a problem occures, ask for reload
            {
                IsReloadNeeded(true);
            }
        }
        private void AdjustMusicPosition(int seconds)
        {
            mediaPlayer.Position = mediaPlayer.Position.Add(TimeSpan.FromSeconds(seconds));
            slider.Value = mediaPlayer.Position.TotalSeconds;
            totaltime = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds - mediaPlayer.Position.TotalSeconds;
            remainedTime.Text = RoutineLogics.TimeFormat(totaltime);
        }

        #endregion

        #region OnClosing functions
        protected override void OnClosing(CancelEventArgs e)
        {
            // When window closed, no longer this variables needed
            ShredTime();
            ShredMediaPlayer();
            ShredTempData();
        }
        private void ShredTempData()
        {
            musicfilter = null;
            totaltime = 0;
        }
        private void ShredTime()
        {
            if (time != null)
            {
                time.Stop();
                time.Tick -= UpdateSliderPosition;
                time = null;
            }
        }
        private void ShredMediaPlayer()
        {
            if (isPaused)
            {
                currentAudio = null;
                if (mediaPlayer != null)
                {
                    mediaPlayer.Close();
                    mediaPlayer = null;
                }
            }
            else if (mediaPlayer != null)
            {
                mediaPlayer.MediaOpened -= InitializeMediaController;
            }
        }
        #endregion

        #region MediaPlayer functions
        public void InitializeMediaController(object sender, EventArgs e)
        {
            InitializeSliderLogics();
        }
        private void InitializeMediaPlayer(string itemname)
        {
            try
            {
                if (mediaPlayer == null) mediaPlayer = new MediaPlayer();

                mediaPlayer.Close();
                mediaPlayer.Open(new Uri(Path.Combine(MainWindow.MusicAppPath, itemname), UriKind.Relative));
                mediaPlayer.MediaOpened += InitializeMediaController;
                mediaPlayer.Play();
                if (currentAudio != null && currentMusic.IsVisible == false)
                {
                    ShowCurrentMusic();
                }
                else if (currentMusic.IsVisible)
                {
                    currentMusic.Text = currentAudio;
                }
                PaintSelectedMusic();
            }
            catch (Exception ex)
            {
                RoutineLogics.ErrorMessage(Errors.OPEN_ERR, Errors.OPEN_ERR_MSG, itemname ?? "null MediaPlayer", "\n", ex.Message);
            }
        }

        #endregion

        #region Panel Sytle functions
        private void ShowCurrentMusic()
        {
            if (currentAudio != null)
            {
                Grid.SetColumnSpan(listbox, 1);
                Grid.SetRowSpan(listbox, 1);
                currentPanel.Visibility = Visibility.Visible;
                musicPanel1.Visibility = Visibility.Visible;
                musicPanel2.Visibility = Visibility.Visible;
                slider.Visibility = Visibility.Visible;
                remainedTime.Visibility = Visibility.Visible;
                SetDisableStyle(startButton);
                SetEnableStyle(stopButton);
                isPaused = false;
            }
            else
            {
                currentPanel.Visibility = Visibility.Collapsed;
                musicPanel1.Visibility = Visibility.Collapsed;
                musicPanel2.Visibility = Visibility.Collapsed;
                slider.Visibility = Visibility.Collapsed;
                remainedTime.Visibility = Visibility.Collapsed;
                Grid.SetColumnSpan(listbox, 2);
                Grid.SetRowSpan(listbox, 4);
            }
        }
        private void SetDisableStyle(Button button)
        {
            button.IsEnabled = false;
            button.Foreground = Brushes.Gray;
        }
        private void SetEnableStyle(Button button)
        {
            button.IsEnabled = true;
            button.Foreground = Brushes.White;
        }
        private void PaintSelectedMusic()
        {
            if (currentAudio != null)
            {
                foreach (ListBoxItem item in listbox.Items)
                {
                    TextBlock itemname = item.Tag as TextBlock;
                    if (itemname.Text.Trim() == currentAudio)
                    {
                        item.Background = Brushes.Gray;
                        itemname.Foreground = new SolidColorBrush(Colors.Black);
                    }
                    else
                    {
                        item.Background = Brushes.Black;
                        itemname.Foreground = new SolidColorBrush(Colors.White);
                    }
                }
            }
            // reset control panel
            SetDisableStyle(startButton);
            SetEnableStyle(stopButton);
        }
        private void CreateMusicItem(string filename, string imagepath, SolidColorBrush textcolor)
        {
            BitmapImage bitmapImage = new BitmapImage(new Uri(imagepath, UriKind.RelativeOrAbsolute));
            bitmapImage.Freeze(); // for higher performance

            TextBlock textblock = new TextBlock
            {
                Text = "  " + filename, // spaces for left margin
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = textcolor
            };

            StackPanel itempanel = new StackPanel { Orientation = Orientation.Horizontal, Height = 25 };
            itempanel.Children.Add(new Image { Source = bitmapImage });
            itempanel.Children.Add(textblock);

            ListBoxItem item = new ListBoxItem { Content = itempanel, Tag = textblock };
            listbox.Items.Add(item); // supported audios
            
        }
        private void SetMenuStyles()
        {
            MenuItem[] menuItems = { AudioItem, AItem1, AItem2, AItem3, AItem4, AItem5, AItem6 };
            RoutineLogics.SetWindowStyles(fileMenu, menuItems);

            Title = AppTitles.APP_MUSIC;
            AItem5.Header = "_Reload " + AppTitles.APP_MUSIC;
            AItem6.Header = "About " + AppTitles.APP_MUSIC;
        }
        #endregion

        #region Music Movement functions
        public void Play(object sender, RoutedEventArgs e)
        {
            SetDisableStyle(startButton);
            SetEnableStyle(stopButton);
            isPaused = false;
            mediaPlayer.Play();
            time.Start();
        }
        public void Pause(object sender, RoutedEventArgs e)
        {
            try
            {
                time.Stop();
                mediaPlayer.Pause();
                isPaused = true;
                SetEnableStyle(startButton);
                SetDisableStyle(stopButton);
            }
            catch (Exception)
            {
                RoutineLogics.ErrorMessage(Errors.OPEN_ERR, Errors.OPEN_ERR_MSG, "null Audio. It may be deleted. Please reload the main desktop.");
            }
        }
        public void Restart(object sender, RoutedEventArgs e)
        {
            try
            {
                SetDisableStyle(startButton);
                SetEnableStyle(stopButton);
                isPaused = false;
                mediaPlayer.Stop();
                mediaPlayer.Play();
                if (!time.IsEnabled) time.Start();
                InitializeSliderLogics();
            }
            catch (Exception)
            {
                RoutineLogics.ErrorMessage(Errors.OPEN_ERR, Errors.OPEN_ERR_MSG, "null Audio. It may be deleted. Please reload the main desktop.");
            }
        }
        public void Back(object sender, RoutedEventArgs e)
        {
            AdjustMusicPosition(-5);
        }
        public void Forward(object sender, RoutedEventArgs e)
        {
            AdjustMusicPosition(5);
        }
        #endregion

        #region Reload Operation functions
        public void ReloadWindow()
        {
            listbox.Items.Clear();
            IEnumerable<string> musiclist = Directory.EnumerateFileSystemEntries(MainWindow.MusicAppPath);
            foreach (string music in musiclist)
            {
                string filename = Path.GetFileName(music);
                SolidColorBrush defaultcolor = new SolidColorBrush(Colors.White);
                if (filename.EndsWith(SupportedFiles.WAV)) CreateMusicItem(filename, ImagePaths.LWAV_IMG, defaultcolor);
                else if (filename.EndsWith(SupportedFiles.MP3)) CreateMusicItem(filename, ImagePaths.LMP3_IMG, defaultcolor);
                else
                {
                    RoutineLogics.ErrorMessage(Errors.UNSUPP_ERR, filename ?? "null File", " is not supported for ", Versions.GOS_VRS, ", removing.\n.wav\n.mp3\n is supported for now.");
                    File.Delete(music);
                }
            }
            listbox.Items.Refresh();
            if (mediaPlayer != null && isPaused == false)
            {
                currentMusic.Text = currentAudio;
                PaintSelectedMusic();
                InitializeSliderLogics();
            }
            ShowCurrentMusic();
        }
        public void IsReloadNeeded(bool yesOrNo)
        {
            reloadNeeded.Visibility = yesOrNo ? Visibility.Visible : Visibility.Collapsed;
            startButton.Visibility = yesOrNo ? Visibility.Collapsed : Visibility.Visible;
            stopButton.Visibility = yesOrNo ? Visibility.Collapsed : Visibility.Visible;
            restartButton.Visibility = yesOrNo ? Visibility.Collapsed : Visibility.Visible;
            back.Visibility = yesOrNo ? Visibility.Collapsed : Visibility.Visible;
            front.Visibility = yesOrNo ? Visibility.Collapsed : Visibility.Visible;
            slider.Visibility = yesOrNo ? Visibility.Collapsed : Visibility.Visible;
            remainedTime.Visibility = yesOrNo ? Visibility.Collapsed : Visibility.Visible;
            if (yesOrNo == false) ShowCurrentMusic();
            else
            {
                Grid.SetRowSpan(listbox, 1);
                musicSliderPanel.Visibility = Visibility.Visible;
                musicPanel1.Visibility = Visibility.Visible;
                musicPanel2.Visibility = Visibility.Visible;
            }
        }
        private void ReloadDesktopNeeded(object sender, RoutedEventArgs e)
        {
            IsReloadNeeded(false);
            ReloadWindow();
        }
        #endregion

        #region Music MenuItem options functions
        private void AddAudio(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog()
            {
                Title = "Add Audio",
                Filter = musicfilter,
                InitialDirectory = MainWindow.CDesktopPath,
                Multiselect = true
            };
            if (dialog.ShowDialog() == true)
            {
                foreach (string file in dialog.FileNames)
                {
                    string filename = Path.GetFileName(file);
                    try
                    {
                        File.Move(file, Path.Combine(MainWindow.MusicAppPath, filename));
                        if (filename.EndsWith(SupportedFiles.WAV)) CreateMusicItem(filename, ImagePaths.LWAV_IMG, new SolidColorBrush(Colors.Green));
                        else if (filename.EndsWith(SupportedFiles.MP3)) CreateMusicItem(filename, ImagePaths.LMP3_IMG, new SolidColorBrush(Colors.Green));
                    }
                    catch (Exception ex)
                    {
                        RoutineLogics.ErrorMessage(Errors.ADD_ERR, Errors.ADD_ERR_MSG, filename ?? "null File", "\n", ex.Message);
                    }
                }
            }
        }
        private void MoveAudio(object sender, RoutedEventArgs e)
        {
            RoutineLogics.MoveAnythingWithQuery("Move Audio", musicfilter, null, MainWindow.MusicAppPath, MainWindow.MusicAppPath, 1);
            ReloadWindow();
        }
        private void CopyAudio(object sender, RoutedEventArgs e)
        {
            RoutineLogics.CopyAnythingWithQuery("Copy Audio", musicfilter, null, MainWindow.MusicAppPath, MainWindow.MusicAppPath);
        }
        private void DeleteAudio(object sender, RoutedEventArgs e)
        {
            RoutineLogics.MoveAnythingWithQuery("Delete Audio", musicfilter, null, MainWindow.MusicAppPath, MainWindow.TrashPath, 3);
            ReloadWindow();
        }
        private void ReloadWindow_Wanted(object sender, RoutedEventArgs e)
        {
            ReloadWindow();
        }
        private void AboutGenmusicPage_Wanted(object sender, RoutedEventArgs e)
        {
            RoutineLogics.ShowAboutWindow("About " + AppTitles.APP_MUSIC, ImagePaths.MSC_IMG, ImagePaths.LMSC_IMG, Versions.MUSIC_VRS, Messages.ABT_DFLT_MSG);
        }
        #endregion

        #region Unclassified public functions
        public void MusicAppButton_Clicked(string musicpath, string musicname)
        {
            if (time != null)
            {
                time.Stop();
                isPaused = true;
            }

            currentAudio = musicname;
            currentMusic.Text = musicname;

            mediaPlayer = new MediaPlayer();
            mediaPlayer.Close();
            mediaPlayer.Open(new Uri(musicpath, UriKind.Relative));
            mediaPlayer.MediaOpened += InitializeMediaController;

            ShowCurrentMusic();
            PaintSelectedMusic();

            if (time != null) time.Start();

            mediaPlayer.Play();
        }
        #endregion

        #region Unclassified private functions
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ListBoxItem item = listbox.SelectedItem as ListBoxItem ?? throw new NullReferenceException();
                TextBlock textblock = item.Tag as TextBlock;
                string itemname = textblock.Text.Trim(); // .Trim in order to remove spaces begin and last
                currentMusic.Text = itemname;
                currentAudio = itemname;
                InitializeMediaPlayer(itemname);
            }
            catch (NullReferenceException)
            {
                // do nothing on null exception
                InitializeSliderLogics();
                stopButton.IsEnabled = true; // to be avoid stop button is disabled forever
            }
        }
        #endregion
    }
}