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
    public partial class GenMusicApp : Window
    {
        public static DispatcherTimer time = new DispatcherTimer();
        public static string currentAudio;
        public static bool isPaused = true;
        public static MediaPlayer mediaPlayer = new MediaPlayer();
        public Dictionary<ListBoxItem, TextBlock> datacontent;
        public string musicfilter = $"WAV Files (*{SupportedFiles.WAV})|*{SupportedFiles.WAV}|MP3 Files (*{SupportedFiles.MP3})|*{SupportedFiles.MP3}";
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
                    RoutineLogics.ErrorMessage($"{filename} is not supported for GenMusic, removing.\n.wav\n.mp3\n is supported for now.", Errors.UNSUPP_ERR);
                    File.Delete(music);
                }
            }
            listbox.Items.Refresh();
            if (mediaPlayer != null && isPaused == false)
            {
                currentMusic.Content = currentAudio;
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
                datacontent.TryGetValue(item, out TextBlock textblock);
                string itemname = textblock.Text.Trim(); // .Trim in order to remove spaces begin and last
                currentMusic.Content = itemname;
                currentAudio = itemname;
                try
                {
                    mediaPlayer.Close();
                    mediaPlayer.Open(new Uri(Path.Combine(MainWindow.MusicAppPath, itemname), UriKind.Relative));
                    mediaPlayer.MediaOpened += InitializeMediaController;
                    mediaPlayer.Play();
                    if (currentAudio != null) ShowCurrentMusic(); 
                    PaintSelectedMusic();
                } catch(Exception ex)
                {
                    RoutineLogics.ErrorMessage($"{Errors.OPEN_ERR_MSG}{itemname ?? "null MediaPlayer"}\n" + ex.Message, Errors.OPEN_ERR);
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
            slider.Visibility = Visibility.Visible;
            startButton.IsEnabled = false;
            isPaused = false;

            InitializeSliderLogics();
        }

        private void PlayMusic(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Play();
            startButton.IsEnabled = false;
            stopButton.IsEnabled = true;
            restartButton.IsEnabled = true;
            isPaused = false;
            time.Start();
        }

        private void StopMusic(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Pause();
            startButton.IsEnabled = true;
            stopButton.IsEnabled = false;
            restartButton.IsEnabled = true;
            isPaused = true;
            time.Stop();
        }

        private void RestartMusic(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Stop();
            mediaPlayer.Play();
            startButton.IsEnabled = false;
            stopButton.IsEnabled = true;
            restartButton.IsEnabled = true;
            isPaused = false;
            slider.Value = 0;
            time.Start();
        }

        private void MusicBack(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Position = mediaPlayer.Position.Add(TimeSpan.FromSeconds(-5)); // 5 second back
            slider.Value = mediaPlayer.Position.TotalSeconds;
        }

        private void MusicFront(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Position = mediaPlayer.Position.Add(TimeSpan.FromSeconds(5)); // 5 second front
            slider.Value = mediaPlayer.Position.TotalSeconds;
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
                try
                {
                    foreach (string file in dialog.FileNames)
                    {
                        string filename = Path.GetFileName(file);
                        File.Move(file, Path.Combine(MainWindow.MusicAppPath, filename));
                        if (filename.EndsWith(SupportedFiles.WAV)) CreateMusicItem(filename, ImagePaths.LWAV_IMG, new SolidColorBrush(Colors.Green));
                        else if (filename.EndsWith(SupportedFiles.MP3)) CreateMusicItem(filename, ImagePaths.LMP3_IMG, new SolidColorBrush(Colors.Green));
                    }
                }
                catch (Exception ex)
                {
                    RoutineLogics.ErrorMessage(Errors.ADD_ERR_MSG + "the file\n" + ex.Message, Errors.ADD_ERR);
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
            slider.Value += 0.52; // 1 second front
        }

        private void SliderPositionChanged(object sender, MouseButtonEventArgs e) // MouseButtonEventArgs = waits until mause events, then do action
        {
            mediaPlayer.Position = TimeSpan.FromSeconds(slider.Value);
        }

        private void InitializeSliderLogics()
        {
            // set the slider's range
            slider.Minimum = 0;
            if (mediaPlayer.NaturalDuration.HasTimeSpan)
            {
                slider.Maximum = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds; // get the total seconds of media player
                slider.Value = mediaPlayer.Position.TotalSeconds; // get the current second of media player
            }
            time.Interval = TimeSpan.FromSeconds(1);
            time.Tick += UpdateSliderPosition;
            time.Start();
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
