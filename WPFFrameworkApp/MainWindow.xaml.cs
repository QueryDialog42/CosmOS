﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WPFFrameworkApp
{
    public partial class MainWindow : Window
    {
        public static string CDesktopPath; // C_DESKTOP folder path
        public static string TempPath; // Temporary path to store currentDesktop path
        public static string MusicAppPath;
        public static string TrashPath;
        public string currentDesktop; // path to unique desktop
        public MainWindow()
        {
            InitializeComponent();
            if (TempPath == null) CheckConfig();
            currentDesktop = TempPath.Trim();
            CDesktopPath = File.ReadAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), Configs.CPATH)).Trim();
            MusicAppPath = Path.Combine(CDesktopPath, HiddenFolders.HAUD_FOL);
            TrashPath = Path.Combine(CDesktopPath, HiddenFolders.HTRSH_FOL);
            if (currentDesktop != null) ReloadDesktop(this, currentDesktop);
        }
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
            catch(Exception e)
            {
                ErrorMessage(Errors.REL_ERR_MSG + "Main Window\n" + e.Message, Errors.REL_ERR);
            }
        }

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
                catch(Exception ex)
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
                string[] options = {"Run", "Delete"};
                switch(QueryDialog.ShowQueryDialog(filename, "Executable File Options", options, ImagePaths.EXE_IMG))
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
                        File.Move(filepath, Path.Combine(TrashPath, filename));
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
                TempPath = Path.Combine(desktopPath, filename);
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
                IEnumerable<string> files = Directory.GetFiles(Path.Combine(CDesktopPath, HiddenFolders.HTRSH_FOL));
                if (files.Any()) return new BitmapImage(new Uri(ImagePaths.FULL_IMG, UriKind.RelativeOrAbsolute));
                else return new BitmapImage(new Uri(ImagePaths.EMPT_IMG, UriKind.RelativeOrAbsolute));
            }
            catch (Exception ex) 
            {
                ErrorMessage($"{Errors.OPEN_ERR_MSG}{HiddenFolders.HTRSH_FOL}\n" + ex.Message, "Trashbacket" + Errors.OPEN_ERR);
                return new BitmapImage(new Uri(ImagePaths.EMPT_IMG, UriKind.RelativeOrAbsolute));
            }
        }
        private void CheckConfig()
        {
            string ConfigFileText = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), Configs.CPATH); //Home Directory\CDesktop.txt
            if (File.Exists(ConfigFileText) == false)
            {
                if (MessageBox.Show($"{Configs.CPATH} could not find. Resetting path needed.", "Path could not find", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                    TempPath = ConfigurePath(ConfigFileText);
                else Environment.Exit(0);
            }
            else
            {
                try
                {
                    TempPath = File.ReadAllText(ConfigFileText);
                    if (Directory.Exists(TempPath) == false)
                    {
                        if (MessageBox.Show($"Path to {Configs.CDESK} is corrupted or does not exists.\nPlease reset the desktop path", "Incorrect path", MessageBoxButton.OKCancel, MessageBoxImage.Error) == MessageBoxResult.OK)
                            TempPath = ConfigurePath(ConfigFileText);
                        else Environment.Exit(0);
                    }
                }
                catch (Exception e)
                {
                    ErrorMessage(Errors.READ_ERR_MSG + "the path from " + Configs.CPATH + "\n" + e.Message, Errors.READ_ERR);
                    Environment.Exit(0);
                }
            }
        }

        private static void ImportFile(MainWindow window, string desktopPath)
        {
            Microsoft.Win32.OpenFileDialog filedialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = $"Text Files (*{SupportedFiles.TXT})|*{SupportedFiles.TXT}|RTF Files (*{SupportedFiles.RTF})|*{SupportedFiles.RTF}|WAV Files (*{SupportedFiles.WAV})|*{SupportedFiles.WAV}|MP3 Files (*{SupportedFiles.MP3})|*{SupportedFiles.MP3}|EXE Files (*{SupportedFiles.EXE})|*{SupportedFiles.EXE}",
                Title = "Import File"
            };

            if (filedialog.ShowDialog() == true)
            {
                string filepath = filedialog.FileName;
                string filename = Path.GetFileName(filepath);
                try
                {
                    File.Move(filepath, Path.Combine(desktopPath, filename));
                    ReloadDesktop(window, desktopPath);
                }
                catch (Exception ex)
                {
                    ErrorMessage($"{Errors.IMP_ERR_MSG}{filename}\n" + ex.Message, Errors.IMP_ERR);
                }
            }
        }

        private void NewFolder(object sender, RoutedEventArgs e)
        {
            try
            {
                string foldername = InputDialog.ShowInputDialog("Please enter the name of the folder:", "New Folder", ImagePaths.NEWFOL_IMG, ImagePaths.LOGO_IMG);
                if (InputDialog.Result == true)
                {
                    string folderpath = Path.Combine(currentDesktop, foldername);
                    if (Directory.Exists(folderpath) == false)
                    {
                        Directory.CreateDirectory(folderpath);
                        ReloadDesktop(this, currentDesktop);
                    }
                    else ErrorMessage($"{foldername} already exists.", Errors.CRT_ERR);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage(Errors.READ_ERR_MSG + "the folder.\n" + ex.Message, Errors.CRT_ERR);
            }
        }

        private void DeleteFolder(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Directory.Exists(currentDesktop) && Path.GetFileName(currentDesktop) != Configs.CDESK)
                {
                    new DirectoryInfo(currentDesktop) { Attributes = FileAttributes.Normal };
                    Directory.Delete(currentDesktop);
                    Close();
                }
                else
                {
                    ErrorMessage($"You cannot delete {Configs.CDESK} folder.", "Permission Error");
                }
            }
            catch (Exception ex)
            {
                ErrorMessage(Errors.DEL_ERR_MSG + "the folder.\n" + ex.Message, Errors.DEL_ERR);
            }
        }
        private string ConfigurePath(string ConfigFileText)
        {
            string input = InputDialog.ShowInputDialog($"Please enter {Configs.CDESK} path", "Path Needed");
            //Resumes until valid path is entered
            while (true)
            {
                if (InputDialog.Result == true)
                {
                    if (input.EndsWith(Configs.CDESK) == false) input = InputDialog.ShowInputDialog($"Path must end with {Configs.CDESK}.\nIf {Configs.CDESK} folder does not exists, create one\nand enter the path of the folder", "Invalid path", ImagePaths.WRNG_IMG);
                    else if (Directory.Exists(input) == false) input = InputDialog.ShowInputDialog($"Path to {Configs.CDESK} does not exists.\nPlease check if the path is correct.", "Incorrect path", ImagePaths.WRNG_IMG);
                    else
                    {
                        try
                        {
                            using (StreamWriter writer = new StreamWriter(File.Create(ConfigFileText)))
                            {
                                writer.WriteLine(input);
                                Directory.CreateDirectory(Path.Combine(input, HiddenFolders.HAUD_FOL));
                                return input;
                            }
                        } catch(Exception e)
                        {
                            ErrorMessage(Errors.WRT_ERR_MSG + " the path to file.\n" + e.Message, Errors.WRT_ERR);
                            Environment.Exit(0);
                        }
                    }
                }
                else
                {
                    switch(MessageBox.Show("If you continue without valid path, your files will not be loaded.\nDo you want to continue?", "OS Without Desktop Path", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning)){
                        case MessageBoxResult.Yes: return null;
                        case MessageBoxResult.No:
                            input = InputDialog.ShowInputDialog($"Please enter {Configs.CDESK} path", "Path Needed");
                            continue;
                        default: Environment.Exit(0); break;

                    }
                }
            }
        }

        #region Subroutines
        private void NoteApp_Clicked(object sender, RoutedEventArgs e)
        {
            NoteAppLogics.NewNote_Wanted(this, currentDesktop);
        }

        private void MusicAppStart(object sender, RoutedEventArgs e)
        {
            if (IsWindowOpen() == false)
            {
               new GenMusicApp();
            }
            else
            {
                foreach (Window window in Application.Current.Windows)
                {
                    if (window is GenMusicApp)
                    {
                        window.Activate();
                    }
                }
            }
        }

        private void ReloadDesktop_Wanted(object sender, RoutedEventArgs e)
        {
            ReloadDesktop(this, currentDesktop);
        }

        private void ImportFile_Wanted(object sender, RoutedEventArgs e)
        {
            ImportFile(this, currentDesktop);
        }

        private void OpenTrashBacket(object sender, RoutedEventArgs e)
        {
            new TrashBacket();
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

        public static void Appearence(Image image, dynamic stackpanel, Button app, TextBlock appname, string logo)
        {
            BitmapImage bitmap = new BitmapImage(new Uri(logo, UriKind.RelativeOrAbsolute));
            bitmap.Freeze();
            image.Source = bitmap;

            stackpanel.Children.Add(image);
            stackpanel.Children.Add(appname);

            app.Content = stackpanel;
        }

        public static void ErrorMessage(string errmessage, string errtitle)
        {
            MessageBox.Show(errmessage, errtitle, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private bool IsWindowOpen()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window is GenMusicApp)
                {
                    return true; // GenMusic is open
                }
            }
            return false; // GenMusic is close
        }
        #endregion
    }
}
