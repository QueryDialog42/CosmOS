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

namespace WPFFrameworkApp
{
    /// <summary>
    /// GenMusicApp.xaml etkileşim mantığı
    /// </summary>
    public partial class GenMusicApp : Window, IDisposable
    {
        public static string currentAudio;
        public static bool isPaused = true;
        public static MediaPlayer mediaPlayer;
        public Dictionary<ListBoxItem, TextBlock> datacontent; // to store items' TextBlock in order to get filename
        public string musicfilter = $"WAV Files (*{SupportedFiles.WAV})|*{SupportedFiles.WAV}|MP3 Files (*{SupportedFiles.MP3})|*{SupportedFiles.MP3}";
        public DispatcherTimer time;
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
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ListBoxItem item = listbox.SelectedItem as ListBoxItem;
                if (item == null) throw new NullReferenceException(); // if no item is selected, do nothing
                datacontent.TryGetValue(item, out TextBlock textblock); // get Textblock of seelcted item
                string itemname = textblock.Text.Trim(); // .Trim in order to remove spaces begin and last
                currentMusic.Text = itemname;
                currentAudio = itemname;
                try
                {
                    if (mediaPlayer == null) mediaPlayer = new MediaPlayer();
                    else mediaPlayer.Close();

                    mediaPlayer.Open(new Uri(Path.Combine(MainWindow.MusicAppPath, itemname), UriKind.Relative));
                    mediaPlayer.MediaOpened += InitializeMediaController;
                    mediaPlayer.Play();
                    if (currentAudio != null) ShowCurrentMusic(); 
                    PaintSelectedMusic();
                } catch(Exception ex)
                {
                    RoutineLogics.ErrorMessage(Errors.OPEN_ERR, Errors.OPEN_ERR_MSG, itemname ?? "null MediaPlayer", "\n", ex.Message);
                }
            } catch (NullReferenceException)
            {
                // do nothing on null exception
                stopButton.IsEnabled = true; // to be avoid stop button is disable forever
            }
        }

        private void InitializeMediaController(object sender, EventArgs e)
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
            musicSliderPanel.Visibility = Visibility.Visible;
            SetDisableStyle(startButton);
            SetEnableStyle(stopButton);
            isPaused = false;

            InitializeSliderLogics();
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
            time.Stop();
            mediaPlayer.Pause();
            isPaused = true;
            SetEnableStyle(startButton);
            SetDisableStyle(stopButton);
        }

        private void RestartMusic(object sender, RoutedEventArgs e)
        {
            SetDisableStyle(startButton);
            SetEnableStyle(stopButton);
            isPaused = false;
            slider.Value = 0;
            mediaPlayer.Stop();
            mediaPlayer.Play();
            if (time.IsEnabled == false) time.Start();

            InitializeSliderLogics();
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

        private void DeleteTimeOnClose(object sender, System.ComponentModel.CancelEventArgs e) // Closing in .xaml file
        {
            Dispose();
        }

        #region Subroutines

        private void ReloadMusicApp_Wanted(object sender, RoutedEventArgs e)
        {
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
            restartButton.IsEnabled = true;
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

        private void UpdateSliderPosition(object sender, EventArgs e)
        {
            slider.Value += 1.035; // 1 second front
            totaltime -= 1.035;
            remainedTime.Text = TimeFormat();
            if (totaltime <= 0)
            {
                time.Stop();
                isPaused = true;
            }
        }

        private void SliderPositionChanged(object sender, MouseButtonEventArgs e) // MouseButtonEventArgs = waits until mause events, then do action
        {
            mediaPlayer.Position = TimeSpan.FromSeconds(slider.Value);
            totaltime = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds - mediaPlayer.Position.TotalSeconds;
            remainedTime.Text = TimeFormat();
        }

        private void InitializeSliderLogics()
        {
            // set the slider's range
            slider.Minimum = 0;
            if (mediaPlayer.NaturalDuration.HasTimeSpan)
            {
                slider.Maximum = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds; // get the total seconds of media player
                slider.Value = mediaPlayer.Position.TotalSeconds; // get the current second of media player
                totaltime = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds - mediaPlayer.Position.TotalSeconds;
                remainedTime.Text = TimeFormat(); //minutes:seconds
            }
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

        public void Dispose()
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

            if (isPaused)
            {
                currentAudio = null;
                if (mediaPlayer != null) mediaPlayer.Close();
                mediaPlayer = null;
            }
        }

        /*
        public static T GetChildOfType<T>(DependencyObject parent) where T : DependencyObject
        {
            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T typedChild)
                {
                    return typedChild;
                }
                var childOfChild = GetChildOfType<T>(child);
                if (childOfChild != null)
                {
                    return childOfChild;
                }
            }
            return null;
        
        }
        */
        #endregion
    }
}
