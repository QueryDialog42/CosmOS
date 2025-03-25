using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using System.Windows.Media;

namespace WPFFrameworkApp
{
    /// <summary>
    /// GenMusicApp.xaml etkileşim mantığı
    /// </summary>
    public partial class GenMusicApp : Window
    {
        public static string currentAudio;
        public static bool isPaused = true;
        public static MediaPlayer mediaPlayer = new MediaPlayer();
        public string musicfilter = $"WAV Files (*{SupportedFiles.WAV})|*{SupportedFiles.WAV}|MP3 Files (*{SupportedFiles.MP3})|*{SupportedFiles.MP3}";
        public GenMusicApp()
        {
            InitializeComponent();
            ReloadMusicApp();
            Show();
        }

        private void ReloadMusicApp()
        {
            listbox.Items.Clear();
            IEnumerable<string> musiclist = Directory.EnumerateFileSystemEntries(MainWindow.MusicAppPath); //C_DESKTOP PATH should be entered!!
            foreach (string music in musiclist)
            {
                string filename = Path.GetFileName(music);
                if (filename.EndsWith(SupportedFiles.WAV) || filename.EndsWith(SupportedFiles.MP3))
                {
                    ListBoxItem item = new ListBoxItem()
                    {
                        Content = filename,
                        FontSize = 15
                    };
                    listbox.Items.Add(item); // supported audios
                }
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
                string itemname = item.Content as string;
                currentMusic.Content = itemname;
                currentAudio = itemname;
                try
                {
                    mediaPlayer.Close();
                    mediaPlayer.Open(new Uri(Path.Combine(MainWindow.MusicAppPath, itemname), UriKind.Relative));
                    mediaPlayer.Play();
                    if (currentAudio != null) ShowCurrentMusic(); 
                    PaintSelectedMusic();
                } catch(Exception ex)
                {
                    RoutineLogics.ErrorMessage($"{Errors.OPEN_ERR_MSG}{itemname ?? "null MediaPlayer"}" + ex.Message, Errors.OPEN_ERR);
                }
            } catch (NullReferenceException)
            {
                // do nothing on null exception
                stopButton.IsEnabled = true; // to be avoid stop button is disable forever
            }
        }

        private void ShowCurrentMusic()
        {
            Grid.SetColumnSpan(listbox, 1);
            Grid.SetRowSpan(listbox, 1);
            currentPanel.Visibility = Visibility.Visible;
            musicPanel1.Visibility = Visibility.Visible;
            musicPanel2.Visibility = Visibility.Visible;
            startButton.IsEnabled = false;
            isPaused = false;
        }

        private void PlayMusic(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Play();
            startButton.IsEnabled = false;
            stopButton.IsEnabled = true;
            restartButton.IsEnabled = true;
            isPaused = false;
        }

        private void StopMusic(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Pause();
            startButton.IsEnabled = true;
            stopButton.IsEnabled = false;
            restartButton.IsEnabled = true;
            isPaused = true;
        }

        private void RestartMusic(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Stop();
            mediaPlayer.Play();
            startButton.IsEnabled = false;
            stopButton.IsEnabled = true;
            restartButton.IsEnabled = true;
            isPaused = false;
        }

        private void MusicBack(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Position = mediaPlayer.Position.Add(TimeSpan.FromSeconds(-5)); // 5 second back
        }

        private void MusicFront(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Position = mediaPlayer.Position.Add(TimeSpan.FromSeconds(5)); // 5 second front
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
                        ListBoxItem item = new ListBoxItem()
                        {
                            Content = filename,
                            FontSize = 15,
                            Foreground = Brushes.Green
                        };
                        listbox.Items.Add(item);
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
                    if ((string)item.Content == currentAudio)
                    {
                        item.Background = Brushes.Gray;
                        item.Foreground = Brushes.Black;
                    }
                    else
                    {
                        item.Background = Brushes.Black;
                        item.Foreground = Brushes.White;
                    }
                }
            }
            // reset control panel
            startButton.IsEnabled = false;
            stopButton.IsEnabled = true;
            restartButton.IsEnabled = true;
        }
        #endregion
    }
}
