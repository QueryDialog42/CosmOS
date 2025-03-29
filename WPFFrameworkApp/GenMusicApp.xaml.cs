using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Windows.Input;
using System.ComponentModel;

namespace WPFFrameworkApp
{
    /// <summary>
    /// GenMusicApp.xaml etkileşim mantığı
    /// </summary>
    public partial class GenMusicApp : Window
    {
        public static string currentAudio;
        public static bool isPaused = true;
        public static bool allowToInitialize = true;
        public static MediaPlayer mediaPlayer;
        public static DispatcherTimer time;
        public Dictionary<ListBoxItem, TextBlock> datacontent; // to store items' TextBlock in order to get filename
        public string musicfilter = $"WAV Files (*{SupportedFiles.WAV})|*{SupportedFiles.WAV}|MP3 Files (*{SupportedFiles.MP3})|*{SupportedFiles.MP3}";
        
        double totaltime;
        public GenMusicApp()
        {
            InitializeComponent();
            ReloadMusicApp();
            Show();
        }

        private void ReloadMusicApp()
        {
            datacontent = new Dictionary<ListBoxItem, TextBlock>(); // to restore item and its textblock
            listbox.Items.Clear();
            IEnumerable<string> musiclist = Directory.EnumerateFileSystemEntries(MainWindow.MusicAppPath); //C_DESKTOP PATH should be entered!
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
                ShowCurrentMusic();
                PaintSelectedMusic();
                allowToInitialize = true;
                InitializeSliderLogics();
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ListBoxItem item = listbox.SelectedItem as ListBoxItem ?? throw new NullReferenceException();
                datacontent.TryGetValue(item, out TextBlock textblock); // get Textblock of seelcted item
                string itemname = textblock.Text.Trim(); // .Trim in order to remove spaces begin and last
                currentMusic.Text = itemname;
                currentAudio = itemname;
                allowToInitialize = true;
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
                } catch (Exception ex)
                { 
                    RoutineLogics.ErrorMessage(Errors.OPEN_ERR, Errors.OPEN_ERR_MSG, itemname ?? "null MediaPlayer", "\n", ex.Message);
                }
            } catch (NullReferenceException)
            {
                // do nothing on null exception
                stopButton.IsEnabled = true; // to be avoid stop button is disable forever
            }
        }

        public void InitializeMediaController(object sender, EventArgs e)
        {
            InitializeSliderLogics();
        }

        private void ShowCurrentMusic()
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

        private void PlayMusic(object sender, RoutedEventArgs e)
        {
            SetDisableStyle(startButton);
            SetEnableStyle(stopButton);
            isPaused = false;
            mediaPlayer.Play();
            time.Start();
        }

        private void StopMusic(object sender, RoutedEventArgs e)
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

        private void RestartMusic(object sender, RoutedEventArgs e)
        {

            try
            {
                SetDisableStyle(startButton);
                SetEnableStyle(stopButton);
                isPaused = false;
                slider.Value = 0;
                mediaPlayer.Stop();
                mediaPlayer.Play();
                if (time.IsEnabled == false) time.Start();
                
                allowToInitialize = true;
                InitializeSliderLogics();
            }
            catch (Exception)
            {
                RoutineLogics.ErrorMessage(Errors.OPEN_ERR, Errors.OPEN_ERR_MSG, "null Audio. It may be deleted. Please reload the main desktop.");
            }
        }

        private void MusicBack(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Position = mediaPlayer.Position.Add(TimeSpan.FromSeconds(-5)); // 5 second back
            slider.Value = mediaPlayer.Position.TotalSeconds;
            totaltime = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds - mediaPlayer.Position.TotalSeconds;
            remainedTime.Text = TimeFormat();
        }

        private void MusicFront(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Position = mediaPlayer.Position.Add(TimeSpan.FromSeconds(5)); // 5 second front
            slider.Value = mediaPlayer.Position.TotalSeconds;
            totaltime = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds - mediaPlayer.Position.TotalSeconds;
            remainedTime.Text = TimeFormat();
        }

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
            ReloadMusicApp();
        }

        private void CopyAudio(object sender, RoutedEventArgs e)
        {
            RoutineLogics.CopyAnythingWithQuery("Copy Audio", musicfilter, null, MainWindow.MusicAppPath, MainWindow.MusicAppPath);
        }

        private void DeleteAudio(object sender, RoutedEventArgs e)
        {
            RoutineLogics.MoveAnythingWithQuery("Delete Audio", musicfilter, null, MainWindow.MusicAppPath, MainWindow.TrashPath, 3);
            ReloadMusicApp();
        }

        private void AboutGenmusicPage_Wanted(object sender, RoutedEventArgs e)
        {
            RoutineLogics.ShowAboutWindow("About GenMusic", ImagePaths.MSC_IMG, ImagePaths.LMSC_IMG, Versions.MUSIC_VRS, Messages.ABT_DFLT_MSG);
        }

        public void MusicAppButton_Clicked(string musicpath, string musicname)
        {
            allowToInitialize = true;
            if (time != null)
            {
                time.Stop();
                isPaused = true;
            }

            currentAudio = musicname;
            currentMusic.Text = musicname;
            
            if (mediaPlayer == null) mediaPlayer = new MediaPlayer();

            mediaPlayer.Close();
            mediaPlayer.Open(new Uri(musicpath, UriKind.Relative));
            mediaPlayer.MediaOpened += InitializeMediaController;
            
            ShowCurrentMusic();
            PaintSelectedMusic();

            mediaPlayer.Play();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            // When window closed, no longer this variables needed
            if (time != null)
            {
                time.Stop();
                time.Tick -= UpdateSliderPosition;
                time = null;
            }

            datacontent = null;
            musicfilter = null;
            totaltime = 0;
            allowToInitialize = false;

            if (isPaused)
            {
                currentAudio = null;
                if (mediaPlayer != null)
                {
                    mediaPlayer.Close();
                    mediaPlayer = null;
                }
            }
        }

        #region Subroutines

        private void ReloadDesktopNeeded(object sender, RoutedEventArgs e)
        {
            IsReloadNeeded(false);
            ReloadMusicApp();
        }

        private void IsReloadNeeded(bool yesOrNo)
        {
            switch (yesOrNo)
            {
                case true:
                    reloadNeeded.Visibility = Visibility.Visible;
                    slider.Visibility = Visibility.Collapsed;
                    remainedTime.Visibility = Visibility.Collapsed;
                    startButton.Visibility = Visibility.Collapsed;
                    stopButton.Visibility = Visibility.Collapsed;
                    restartButton.Visibility = Visibility.Collapsed;
                    back.Visibility = Visibility.Collapsed;
                    front.Visibility = Visibility.Collapsed;
                    break;
                case false:
                    reloadNeeded.Visibility = Visibility.Collapsed;
                    slider.Visibility = Visibility.Visible;
                    remainedTime.Visibility= Visibility.Visible;
                    startButton.Visibility= Visibility.Visible;
                    stopButton.Visibility= Visibility.Visible;
                    restartButton.Visibility= Visibility.Visible;
                    back.Visibility= Visibility.Visible;
                    front.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void ReloadMusicApp_Wanted(object sender, RoutedEventArgs e)
        {
            IsReloadNeeded(false);
            ReloadMusicApp();
        }

        private void PaintSelectedMusic()
        {
            if (currentAudio != null && isPaused == false)
            {
                foreach (ListBoxItem item in listbox.Items)
                {
                    datacontent.TryGetValue(item, out TextBlock itemname);
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
            startButton.IsEnabled = false;
            stopButton.IsEnabled = true;
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

            ListBoxItem item = new ListBoxItem { Content = itempanel };
            listbox.Items.Add(item); // supported audios
            datacontent.Add(item, textblock);
        }

        public void UpdateSliderPosition(object sender, EventArgs e)
        {
            slider.Value += 1.035; // 1 second front
            totaltime -= 1.035;
            remainedTime.Text = TimeFormat();
            if (totaltime <= 0)
            {
                time.Stop();
                isPaused = true;
            }
            Console.WriteLine(1);
        }

        private void SliderPositionChanged(object sender, MouseButtonEventArgs e) // MouseButtonEventArgs = waits until mause events, then do action
        {
            mediaPlayer.Position = TimeSpan.FromSeconds(slider.Value);
            totaltime = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds - mediaPlayer.Position.TotalSeconds;
            remainedTime.Text = TimeFormat();
        }

        private void InitializeSliderLogics()
        {
            if (mediaPlayer.NaturalDuration.HasTimeSpan && allowToInitialize) // slider starts with -1 minimum value, if minimum value is not -1, then slider is setted
            {   
                slider.Minimum = 0;
                slider.Maximum = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds; // get the total seconds of media player
                slider.Value = mediaPlayer.Position.TotalSeconds; // get the current second of media player
                totaltime = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds - mediaPlayer.Position.TotalSeconds;
                remainedTime.Text = TimeFormat(); //minutes:seconds

                if (time == null)
                {
                    time = new DispatcherTimer();
                    time.Tick += UpdateSliderPosition;
                    time.Interval = TimeSpan.FromSeconds(1);
                    time.Start();
                }
                else if (time.IsEnabled == false)
                {
                    time.Start();
                }
                allowToInitialize = false;
            }
            else
            {
                IsReloadNeeded(true);
            }
        }

        private string TimeFormat()
        {
            int minutes = (int)(totaltime / 60);
            int seconds = (int)(totaltime % 60);

            return string.Format("{0:00}:{1:00}", minutes, seconds);
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
        #endregion
    }
}
