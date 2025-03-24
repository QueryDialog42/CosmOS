using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WPFFrameworkApp
{
    public static class RoutineLogics
    {
        public static void ReloadDesktop(MainWindow window, string desktopPath)
        {
            window.desktop.Children.Clear();
            window.folderdesktop.Children.Clear();
            try
            {
                string[] hiddenfolders = { HiddenFolders.HAUD_FOL, HiddenFolders.HTRSH_FOL };
                IEnumerable<string> files = Directory.EnumerateFileSystemEntries(desktopPath);
                foreach (string file in files)
                {
                    string filename = Path.GetFileName(file);
                    Button app = CreateButton(filename);
                    TextBlock appname = CreateTextBlock(filename);
                    Image image = CreateImage();
                    StackPanel stackpanel = new StackPanel
                    {
                        Orientation = Orientation.Vertical
                    };

                    if (Directory.Exists(file))
                    {
                        if (hiddenfolders.Contains(filename) == false) InitFolder(window, image, stackpanel, app, appname, desktopPath, filename);
                        else
                        {
                            Grid.SetColumnSpan(window.safari, 1);
                            window.trashApp.Visibility = Visibility.Visible;
                            window.trashimage.Source = InitTrashBacket();
                        }
                    }
                    else if (filename.EndsWith(SupportedFiles.TXT)) InitTextFile(window, image, stackpanel, app, appname, desktopPath, filename);
                    else if (filename.EndsWith(SupportedFiles.RTF)) InitRTFFile(window, image, stackpanel, app, appname, desktopPath, filename);
                    else if (filename.EndsWith(SupportedFiles.WAV)) InitAudioFile(window, image, stackpanel, app, appname, file, filename, ImagePaths.WAV_IMG);
                    else if (filename.EndsWith(SupportedFiles.MP3)) InitAudioFile(window, image, stackpanel, app, appname, file, filename, ImagePaths.MP3_IMG);
                    else if (filename.EndsWith(SupportedFiles.EXE)) InitEXEFile(window, image, stackpanel, app, appname, desktopPath, file, filename);
                    else
                    {
                        ErrorMessage($"{filename} is not supported for GencOS now.", Errors.UNSUPP_ERR);
                        File.Delete(file);
                    }
                }
                window.Show();
            }
            catch (Exception e)
            {
                ErrorMessage(Errors.REL_ERR_MSG + "Main Window\n" + e.Message, Errors.REL_ERR);
            }
        }
        public static void ErrorMessage(string errmessage, string errtitle)
        {
            MessageBox.Show(errmessage, errtitle, MessageBoxButton.OK, MessageBoxImage.Error);
        }
        public static void MoveAnythingWithQuery(
            string title,
            string filter,
            string selectedFileName,
            string initialDirectory,
            string currentDesktop,
            short toWhere)
        {
            switch (toWhere)
            {
                case 1:
                    MoveSomeWhere(title, filter, selectedFileName, initialDirectory, currentDesktop);
                    break;
                case 2:
                    MoveCertainWindow(title, filter, initialDirectory, currentDesktop, MainWindow.MusicAppPath);
                    break;
                case 3:
                    MoveCertainWindow(title, filter, initialDirectory, currentDesktop, MainWindow.TrashPath);
                    break;
                case 4:
                    MoveCertainWindow(title, filter, initialDirectory, currentDesktop, MainWindow.CDesktopPath);
                    break;
            }
        }

        public static void MoveAnythingWithoutQuery(
            string currentDesktop,
            string filename,
            string pathToDirection) // path to the file will go
        {
            try
            {
                File.Move(Path.Combine(currentDesktop, filename), pathToDirection);
            }
            catch (Exception e)
            {
                ErrorMessage(Errors.MOVE_ERR_MSG + $"{filename ?? "null File"}\n", Errors.MOVE_ERR);
            }
        }

        public static void CopyAnythingWithQuery(
            string title,
            string filter,
            string selectedFileName,
            string initialDirectory,
            string currentDesktop)
        {
            SaveFileDialog copydialog = new SaveFileDialog
            {
                Title = title,
                Filter = filter,
                FileName = selectedFileName,
                InitialDirectory = initialDirectory,
            };

            if (copydialog.ShowDialog() == true)
            {
                string filepath = copydialog.FileName;
                string filename = Path.GetFileName(filepath); // filepath = the path where the file will go
                try
                {
                    File.Copy(Path.Combine(currentDesktop, filename), filepath);
                }
                catch (Exception ex)
                {
                    ErrorMessage(Errors.COPY_ERR_MSG + $"{filename ?? "null File"}\n" + ex.Message, Errors.COPY_ERR);
                }
            }
        }

        #region Subroutines
        private static void InitTextFile(MainWindow window, Image image, dynamic stackpanel, Button app, TextBlock appname, string desktopPath, string filename)
        {
            Appearence(image, stackpanel, app, appname, ImagePaths.TXT_IMG);

            window.desktop.Children.Add(app);

            app.Click += (sender, e) =>
            {
                try
                {
                    TXTNote noteapp = new TXTNote
                    {
                        windowForNote = window,
                        currentDesktopForNote = desktopPath,
                        Title = filename
                    };
                    noteapp.note.Text = File.ReadAllText(Path.Combine(desktopPath, filename));
                }
                catch (Exception ex)
                {
                    ErrorMessage(Errors.READ_ERR_MSG + "the file\n" + ex.Message, "TXT" + Errors.READ_ERR);
                }
            };
        }

        private static void InitRTFFile(MainWindow window, Image image, dynamic stackpanel, Button app, TextBlock appname, string desktopPath, string filename)
        {
            Appearence(image, stackpanel, app, appname, ImagePaths.RTF_IMG);

            window.desktop.Children.Add(app);

            app.Click += (sender, e) =>
            {
                RTFNote noteapp = new RTFNote
                {
                    windowForNote = window,
                    currentDesktopForNote = desktopPath,
                    Title = filename
                };
                try
                {
                    string rtfContent = File.ReadAllText(Path.Combine(desktopPath, filename));
                    TextRange textRange = new TextRange(noteapp.RichNote.Document.ContentStart, noteapp.RichNote.Document.ContentEnd);

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (StreamWriter writer = new StreamWriter(memoryStream))
                        {
                            writer.Write(rtfContent);
                            writer.Flush();
                            memoryStream.Position = 0; // Reset the stream position

                            textRange.Load(memoryStream, DataFormats.Rtf);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage(Errors.READ_ERR_MSG + "the file\n" + ex.Message, "RTF" + Errors.OPEN_ERR);
                }
            };
        }
        private static void InitAudioFile(MainWindow window, Image image, dynamic stackpanel, Button app, TextBlock appname, string filepath, string filename, string fileimage)
        {
            Appearence(image, stackpanel, app, appname, fileimage);

            window.desktop.Children.Add(app);

            app.Click += (o, e) =>
            {
                GenMusicApp.mediaPlayer.Close();
                GenMusicApp.currentAudio = filename;
                GenMusicApp.mediaPlayer = new MediaPlayer();
                new GenMusicApp();
                try
                {
                    GenMusicApp.mediaPlayer.Open(new Uri(filepath, UriKind.Relative));
                    GenMusicApp.mediaPlayer.Play();
                }
                catch (Exception ex)
                {
                    ErrorMessage(Errors.READ_ERR_MSG + "the file\n" + ex.Message, "Audio" + Errors.OPEN_ERR);
                }
            };
        }

        private static void InitEXEFile(MainWindow window, Image image, dynamic stackpanel, Button app, TextBlock appname, string desktopPath, string filepath, string filename)
        {
            Appearence(image, stackpanel, app, appname, ImagePaths.EXE_IMG);

            window.desktop.Children.Add(app);

            app.Click += (s, e) =>
            {
                string[] options = { "Run", "Move", "Copy", "Delete" };
                switch (QueryDialog.ShowQueryDialog(filename, "Executable File Options", options, ImagePaths.EXE_IMG))
                {
                    case 0:
                        Process process = new Process();
                        process.StartInfo.FileName = filepath;
                        process.StartInfo.CreateNoWindow = true;
                        try
                        {
                            process.Start();
                            process.WaitForExit();
                        }
                        catch (Exception ex)
                        {
                            ErrorMessage($"{Errors.RUN_ERR_MSG}{filename}.\n" + ex.Message, Errors.RUN_ERR);
                        }
                        break;
                    case 1:
                        MoveAnythingWithQuery("Move Exe File", "EXE Files (*.exe)|*.exe", filename, desktopPath, desktopPath, 1);
                        ReloadDesktop(window, desktopPath);
                        break;
                    case 2:
                        CopyAnythingWithQuery("Copy Exe File", "EXE Files (*.exe)|*.exe", filename, desktopPath, desktopPath);
                        ReloadDesktop(window, desktopPath);
                        break;
                    case 3:
                        File.Move(filepath, Path.Combine(MainWindow.TrashPath, filename));
                        ReloadDesktop(window, desktopPath);
                        break;
                }
            };
        }

        private static void InitFolder(MainWindow window, Image image, StackPanel stackpanel, Button app, TextBlock appname, string desktopPath, string filename)
        {
            Appearence(image, stackpanel, app, appname, ImagePaths.FOLDER_IMG);

            window.folderdesktop.Children.Add(app);

            app.Click += (sender, e) =>
            {
                MainWindow.TempPath = Path.Combine(desktopPath, filename);
                MainWindow newWindow = new MainWindow
                {
                    Title = filename
                };
            };
        }

        private static ImageSource InitTrashBacket()
        {
            try
            {
                IEnumerable<string> files = Directory.GetFiles(Path.Combine(MainWindow.CDesktopPath, HiddenFolders.HTRSH_FOL));
                if (files.Any()) return new BitmapImage(new Uri(ImagePaths.FULL_IMG, UriKind.RelativeOrAbsolute));
                else return new BitmapImage(new Uri(ImagePaths.EMPT_IMG, UriKind.RelativeOrAbsolute));
            }
            catch (Exception ex)
            {
                ErrorMessage($"{Errors.OPEN_ERR_MSG}{HiddenFolders.HTRSH_FOL}\n" + ex.Message, "Trashbacket" + Errors.OPEN_ERR);
                return new BitmapImage(new Uri(ImagePaths.EMPT_IMG, UriKind.RelativeOrAbsolute));
            }
        }

        public static void Appearence(Image image, dynamic stackpanel, Button app, TextBlock appname, string logo)
        {
            BitmapImage bitmap = new BitmapImage(new Uri(logo, UriKind.RelativeOrAbsolute));
            bitmap.Freeze();
            image.Source = bitmap;

            stackpanel.Children.Add(image);
            stackpanel.Children.Add(appname);

            app.Content = stackpanel;
        }

        public static Button CreateButton(string filename)
        {
            return new Button()
            {
                Width = 80,
                Height = 80,
                Background = Brushes.Transparent,
                BorderBrush = Brushes.Transparent,
                ToolTip = filename
            };
        }

        public static TextBlock CreateTextBlock(string filename)
        {
            return new TextBlock()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Text = filename
            };
        }

        public static Image CreateImage()
        {
            return new Image
            {
                Width = 60, // Set desired width
                Height = 60, // Set desired height
            };
        }

        private static void MoveSomeWhere(
            string title,
            string filter,
            string selectedFileName,
            string initialDirectory,
            string currentDesktop)
        {
            SaveFileDialog movedialog = new SaveFileDialog
            {
                Title = title,
                Filter = filter,
                FileName = selectedFileName,
                InitialDirectory = initialDirectory,
            };

            if (movedialog.ShowDialog() == true)
            {
                string filepath = movedialog.FileName;
                string filename = Path.GetFileName(filepath); // filepath = the path where the file will go
                try
                {
                    File.Move(Path.Combine(currentDesktop, filename), filepath);
                }
                catch (Exception ex)
                {
                    ErrorMessage(Errors.MOVE_ERR_MSG + $"{filename ?? "null File"}\n" + ex.Message, Errors.MOVE_ERR);
                }
            }
        }
        private static void MoveCertainWindow(
            string title,
            string filter,
            string initialDirectory,
            string currentDesktop,
            string window)
        {
            OpenFileDialog movedialog = new OpenFileDialog
            {
                Title = title,
                Filter = filter,
                InitialDirectory = initialDirectory,
                Multiselect = true
            };

            if (movedialog.ShowDialog() == true)
            {
                foreach (string filepath in movedialog.FileNames)
                {
                    string filename = Path.GetFileName(filepath);
                    try
                    {
                        File.Move(filepath, Path.Combine(window, filename));
                    }
                    catch (Exception ex)
                    {
                        ErrorMessage(Errors.MOVE_ERR_MSG + $"{filename}\n" + ex.Message, Errors.MOVE_ERR);
                    }
                }
            }
        }
        #endregion
    }
}
