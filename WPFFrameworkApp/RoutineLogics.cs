using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WPFFrameworkApp
{
    public partial class RoutineLogics
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
                    if (Directory.Exists(file))
                    {
                        if (hiddenfolders.Contains(filename) == false) InitFolder(window, desktopPath, filename);
                        else // then it is trashbacket
                        {
                            Grid.SetColumnSpan(window.safari, 1);
                            window.trashApp.Visibility = Visibility.Visible;
                            window.trashimage.Source = InitTrashBacket();
                        }
                    }
                    else if (filename.EndsWith(SupportedFiles.TXT)) InitTextFile(window, desktopPath, filename);
                    else if (filename.EndsWith(SupportedFiles.RTF)) InitRTFFile(window, desktopPath, filename);
                    else if (filename.EndsWith(SupportedFiles.WAV)) InitAudioFile(window, file, ImagePaths.WAV_IMG);
                    else if (filename.EndsWith(SupportedFiles.MP3)) InitAudioFile(window, file, ImagePaths.MP3_IMG);
                    else if (filename.EndsWith(SupportedFiles.EXE)) InitEXEFile(window, desktopPath, file, filename);
                    else
                    {
                        ErrorMessage(Errors.UNSUPP_ERR, filename, " is not supported for ", Versions.GOS_VRS);
                        File.Delete(file);
                    }
                }
                window.Show();
            }
            catch (Exception e)
            {
                ErrorMessage(Errors.REL_ERR, Errors.REL_ERR_MSG, "Main Window\n", e.Message);
            }
        }
        public static void ErrorMessage(string errtitle, params string[] errmessage)
        {
            StringBuilder stringbuilder = new StringBuilder();
            foreach (string str in errmessage)
            {
                stringbuilder.Append(str);
            }
            MessageBox.Show(stringbuilder.ToString(), errtitle, MessageBoxButton.OK, MessageBoxImage.Error);
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
                ErrorMessage(Errors.MOVE_ERR, Errors.MOVE_ERR_MSG, filename ?? "null File", "\n", e.Message);
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
                if (filepath.Contains(HiddenFolders.HAUD_FOL) == false && filepath.Contains(HiddenFolders.HTRSH_FOL) == false)
                {
                    try
                    {
                        File.Copy(Path.Combine(currentDesktop, filename), filepath);
                    }
                    catch (Exception ex)
                    {
                        ErrorMessage(Errors.COPY_ERR, Errors.COPY_ERR_MSG, filename ?? "null File", "\n", ex.Message);
                    }
                }
                else
                {
                    ErrorMessage(Errors.PRMS_ERR, "You can not move or copy files into hidden folders manually");
                }
            }
        }
        public static void ShowAboutWindow(string title, string image, string icon, string version, string message)
        {
            AboutWindow aboutwindow = new AboutWindow
            {
                Title = title
            };
            aboutwindow.WhatAbout.Source = new BitmapImage(new Uri(image, UriKind.RelativeOrAbsolute)); ;
            aboutwindow.Icon = new BitmapImage(new Uri(icon, UriKind.RelativeOrAbsolute));
            aboutwindow.Version.Text = version;
            aboutwindow.AboutMessage.Text = message;
        }
        public static void RenameFile_Wanted(string filepath, string ImagePath, string icon = ImagePaths.RENM_IMG)
        {
            string filename = Path.GetFileName(filepath);
            string currentDesktop = Path.GetDirectoryName(filepath);
            
            string input = InputDialog.ShowInputDialog("Enter the new name:", "Rename File", ImagePath, icon);
            if (string.IsNullOrEmpty(input))
            {
                ErrorMessage(Errors.PRMS_ERR, "You can not rename your file as null");
                return;
            }
            string newfilename = input.ToLower();
            string pathToDirection = Path.Combine(currentDesktop, newfilename);

            if (filename.EndsWith(SupportedFiles.TXT))
            {
                if (newfilename.EndsWith(SupportedFiles.TXT) == false && newfilename.EndsWith(SupportedFiles.RTF) == false) // TXT to RTF is supported
                {
                    ErrorMessage(Errors.PRMS_ERR, "You can not rename", filename, " as a non-TXT file except RTF file");
                    return;
                }
            }
            else if (filename.EndsWith(SupportedFiles.RTF))
            {
                if (IsRenameAllowed(filename, newfilename, SupportedFiles.RTF) == false) 
                { 
                    ErrorMessage(Errors.PRMS_ERR, "You can not rename ", filename, " as a non-RTF file");
                    return;
                } 
            }
            else if (filename.EndsWith(SupportedFiles.WAV))
            {
                if (IsRenameAllowed(filename, newfilename, SupportedFiles.WAV) == false) 
                {
                    ErrorMessage(Errors.PRMS_ERR, "You can not rename ", filename, " as a non-WAV file");
                    return;
                }
            }
            else if (filename.EndsWith(SupportedFiles.MP3))
            {
                if (IsRenameAllowed(filename, newfilename, SupportedFiles.MP3) == false) 
                {
                    ErrorMessage(Errors.PRMS_ERR, "You can not rename ", filename, " as a non-MP3 file");
                    return;
                }
            }
            else if (filename.EndsWith(SupportedFiles.EXE))
            {
                if (IsRenameAllowed(filename, newfilename, SupportedFiles.EXE) == false) 
                {
                    ErrorMessage(Errors.PRMS_ERR, "You can not rename ", filename, " as a non-EXE file");
                    return;
                }
            }
            MoveAnythingWithoutQuery(currentDesktop, filename, pathToDirection); // if this function works, then no error occured
        }
        #region Subroutines
        private static bool IsRenameAllowed(string filename, string newfilename, string checkfor) => filename.EndsWith(checkfor) && newfilename.EndsWith(checkfor);
        private static void InitTextFile(
            MainWindow window,
            string desktopPath,
            string filename)
        {
            Button app = CreateButton(filename);
            TextBlock appname = CreateTextBlock(filename);
            Image image = CreateImage();
            StackPanel stackpanel = new StackPanel
            {
                Orientation = Orientation.Vertical
            };
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
                    using (StreamReader reader = new StreamReader(Path.Combine(desktopPath, filename)))
                    {
                        StringBuilder stringbuilder = new StringBuilder();
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            stringbuilder.AppendLine(line);
                        }
                        noteapp.note.Text = stringbuilder.ToString();
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage("TXT" + Errors.READ_ERR, Errors.READ_ERR_MSG, filename ?? "null File", "\n", ex.Message);
                }
            };
        }
        private static void InitRTFFile(
            MainWindow window,
            string desktopPath,
            string filename)
        {
            Button app = CreateButton(filename);
            TextBlock appname = CreateTextBlock(filename);
            Image image = CreateImage();
            StackPanel stackpanel = new StackPanel
            {
                Orientation = Orientation.Vertical
            };
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
                    using (StreamReader reader = new StreamReader(Path.Combine(desktopPath, filename)))
                    {
                        StringBuilder stringbuilder = new StringBuilder();
                        string line;
                        while((line = reader.ReadLine()) != null)
                        {
                            stringbuilder.Append(line);
                        }
                        TextRange textRange = new TextRange(noteapp.RichNote.Document.ContentStart, noteapp.RichNote.Document.ContentEnd);

                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            using (StreamWriter writer = new StreamWriter(memoryStream))
                            {
                                writer.Write(stringbuilder);
                                writer.Flush();
                                memoryStream.Position = 0; // Reset the stream position

                                textRange.Load(memoryStream, DataFormats.Rtf);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage("RTF" + Errors.OPEN_ERR, Errors.READ_ERR_MSG, filename ?? "null File", "\n", ex.Message);
                }
            };
        }
        private static void InitAudioFile(
            MainWindow window,
            string filepath,
            string fileimage)
        {
            string filename = Path.GetFileName(filepath);
            Button app = CreateButton(filename);
            TextBlock appname = CreateTextBlock(filename);
            Image image = CreateImage();
            StackPanel stackpanel = new StackPanel
            {
                Orientation = Orientation.Vertical
            };

            Appearence(image, stackpanel, app, appname, fileimage);

            window.desktop.Children.Add(app);

            app.Click += (o, e) =>
            {
                short result;
                if (filename.EndsWith(SupportedFiles.WAV)) result = AudioOptions(filename, "WAV", fileimage);
                else result = AudioOptions(filename, "MP3", fileimage);

                string currentdesktop = Path.GetDirectoryName(filepath);
                switch(result)
                {
                    case 0:
                        CloseAllGenMusicApps();
                        GenMusicApp.mediaPlayer?.Close();
                        GenMusicApp.mediaPlayer = null;
                        GenMusicApp.isPaused = true;
                        GenMusicApp musicapp = new GenMusicApp();
                        musicapp.MusicAppButton_Clicked(filepath, filename);
                        break;
                    case 1:
                        RenameFile_Wanted(filepath, ImagePaths.WAV_IMG);
                        ReloadDesktop(window, currentdesktop);
                        break;
                    case 2:
                        MoveAnythingWithoutQuery(currentdesktop, filename, Path.Combine(MainWindow.MusicAppPath, filename));
                        ReloadDesktop(window, currentdesktop);
                        break;
                    case 3:
                        MoveAnythingWithoutQuery(currentdesktop, filename, Path.Combine(MainWindow.TrashPath, filename));
                        ReloadDesktop(window, currentdesktop);
                        break;
                }
            };
        }
        private static void InitEXEFile(
            MainWindow window,
            string desktopPath,
            string filepath,
            string filename)
        {
            Button app = CreateButton(filename);
            TextBlock appname = CreateTextBlock(filename);
            Image image = CreateImage();
            StackPanel stackpanel = new StackPanel
            {
                Orientation = Orientation.Vertical
            };
            Appearence(image, stackpanel, app, appname, ImagePaths.EXE_IMG);

            window.desktop.Children.Add(app);

            app.Click += (s, e) =>
            {
                string[] options = { "Run", "Rename", "Move", "Copy", "Delete" };
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
                        RenameFile_Wanted(filepath, ImagePaths.EXE_IMG);
                        ReloadDesktop(window, desktopPath);
                        break;
                    case 2:
                        MoveAnythingWithQuery("Move Exe File", "EXE Files (*.exe)|*.exe", filename, desktopPath, desktopPath, 1);
                        ReloadDesktop(window, desktopPath);
                        break;
                    case 3:
                        CopyAnythingWithQuery("Copy Exe File", "EXE Files (*.exe)|*.exe", filename, desktopPath, desktopPath);
                        ReloadDesktop(window, desktopPath);
                        break;
                    case 4:
                        File.Move(filepath, Path.Combine(MainWindow.TrashPath, filename));
                        ReloadDesktop(window, desktopPath);
                        break;
                }
            };
        }
        private static void InitFolder(
            MainWindow window,
            string desktopPath,
            string filename)
        {
            Button app = CreateButton(filename);
            TextBlock appname = CreateTextBlock(filename);
            Image image = CreateImage();
            StackPanel stackpanel = new StackPanel
            {
                Orientation = Orientation.Vertical
            };
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
                ErrorMessage("Trashbacket" + Errors.OPEN_ERR, Errors.OPEN_ERR_MSG, HiddenFolders.HTRSH_FOL, "\n", ex.Message);
                return new BitmapImage(new Uri(ImagePaths.EMPT_IMG, UriKind.RelativeOrAbsolute));
            }
        }
        public static void Appearence(
            Image image,
            StackPanel stackpanel,
            Button app,
            TextBlock appname,
            string logo)
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
                ToolTip = filename,
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
                if (filepath.Contains(HiddenFolders.HAUD_FOL) == false && filepath.Contains(HiddenFolders.HTRSH_FOL) == false)
                {
                    try
                    {
                        File.Move(Path.Combine(currentDesktop, filename), filepath);
                    }
                    catch (Exception ex)
                    {
                        ErrorMessage(Errors.MOVE_ERR, Errors.MOVE_ERR_MSG, filename ?? "null File", "\n", ex.Message);
                    }
                }
                else
                {
                    ErrorMessage(Errors.PRMS_ERR, "You can not move or copy files into hidden folders manually");
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
                        ErrorMessage(Errors.MOVE_ERR, Errors.MOVE_ERR_MSG, filename ?? "null File", "\n", ex.Message);
                    }
                }
            }
        }
        private static short AudioOptions(string appname, string type, string fileimage)
        {
            string[] options = {"Play", "Rename", "Add to GenMusic", "Delete" };
            return QueryDialog.ShowQueryDialog(appname, type + " File Options", options, fileimage);
        }
        public static bool IsMusicAppOpen()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window is GenMusicApp)
                {
                    window.Activate(); // GenMusic is open
                    return true;
                }
            }
            return false; // GenMusic is close
        }
        public static bool IsTrashBacketOpen()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window is TrashBacket)
                {
                    window.Activate();
                    return true;
                }
            }
            return false;
        }
        public static bool IsMailAppOpen()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window is GenMailApp)
                {
                    window.Activate();
                    return true;
                }
            }
            return false;
        }
        private static void CloseAllGenMusicApps()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window is GenMusicApp musicapp)
                {
                    window.Close();  
                }
            }
        }
        #endregion
    }
}
