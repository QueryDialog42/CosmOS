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
            if (currentAudio != null)
            {
                currentMusic.Content = currentAudio;
                ShowCurrentMusic();
            }
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
                    };
                    listbox.Items.Add(item); // supported audios
                }
                else
                {
                    MainWindow.ErrorMessage($"{filename} is not supported for GenMusic, removing.\n.wav\n.mp3\n is supported for now.", Errors.UNSUPP_ERR);
                    File.Delete(music);
                }
            }
            listbox.Items.Refresh();
            PaintSelectedMusic();
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
                    MainWindow.ErrorMessage($"{Errors.OPEN_ERR_MSG}{itemname ?? "null MediaPlayer"}" + ex.Message, Errors.OPEN_ERR);
                }
            } catch (NullReferenceException)
            {
                // do nothing on null exception
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
        }

        private void PlayMusic(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Play();
            startButton.IsEnabled = false;
            stopButton.IsEnabled = true;
            restartButton.IsEnabled = true;
        }

        private void StopMusic(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Pause();
            startButton.IsEnabled = true;
            stopButton.IsEnabled = false;
            restartButton.IsEnabled = true;
        }

        private void RestartMusic(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Stop();
            mediaPlayer.Play();
            startButton.IsEnabled = false;
            stopButton.IsEnabled = true;
            restartButton.IsEnabled = true;
        }

        private void MusicBack(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Position = mediaPlayer.Position.Add(TimeSpan.FromSeconds(-5));
        }

        private void MusicFront(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Position = mediaPlayer.Position.Add(TimeSpan.FromSeconds(5));
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
                        };
                        listbox.Items.Add(item);
                    }
                }
                catch (Exception ex)
                {
                    MainWindow.ErrorMessage(Errors.ADD_ERR_MSG + "the file\n" + ex.Message, Errors.ADD_ERR);
                }
            }
        }

        private void MoveAudio(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Title = "Move Audio",
                Filter = musicfilter,
                InitialDirectory = MainWindow.MusicAppPath,
                Multiselect = true
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    foreach (string filepath in dialog.FileNames)
                    { 
                        File.Move(filepath, Path.Combine(MainWindow.CDesktopPath, Path.GetFileName(filepath)));
                    }
                } catch (Exception ex)
                {
                    MainWindow.ErrorMessage(Errors.DEL_ERR_MSG + "the audio\n" + ex.Message, Errors.DEL_ERR);
                }
            } 
        }

        private void CopyAudio(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                Title = "Copy Audio",
                Filter = musicfilter,
                InitialDirectory = MainWindow.MusicAppPath
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    string filepath = dialog.FileName;
                    File.Copy(filepath, Path.Combine(MainWindow.CDesktopPath, Path.GetFileName(filepath)));
                } catch (Exception ex)
                {
                    MainWindow.ErrorMessage(Errors.DEL_ERR_MSG + "the audio\n" + ex.Message, Errors.DEL_ERR);
                }
            }
        }

        private void DeleteAudio(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog()
            {
                Title = "Delete Audio",
                Filter = musicfilter,
                InitialDirectory = MainWindow.MusicAppPath,
                Multiselect = false
            };
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    string filepath = dialog.FileName;
                    File.Move(filepath, Path.Combine(MainWindow.TrashPath, Path.GetFileName(filepath)));
                    listbox.Items.Remove(Path.GetFileName(filepath));
                    listbox.Items.Refresh();
                }
                catch (Exception ex)
                {
                    MainWindow.ErrorMessage(Errors.DEL_ERR_MSG + "the audio\n" + ex.Message, Errors.DEL_ERR);
                }
            }
        }

        #region Subroutines

        public bool IsWindowOpen()
        {
            foreach (Window window in Application.Current.Windows)
            {
                Console.WriteLine(window);
                if (window is GenMusicApp)
                {
                    return true;
                }
            }
            return false;
        }

        private void ReloadMusicApp_Wanted(object sender, RoutedEventArgs e)
        {
            ReloadMusicApp();
        }

        private void PaintSelectedMusic()
        {
            if (currentAudio != null)
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
        }
        #endregion
    }
}
