using System;
using System.IO;
using System.Linq;
using System.Windows;
using WPFFrameworkApp2;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Collections.Generic;

namespace WPFFrameworkApp
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer clocktimer;

        public string currentDesktop; // path to unique desktop

        public static bool LoggedIn = false;
        public static string TempPath; // Temporary path to store currentDesktop path
        public static string TrashPath;
        public static string MusicAppPath;
        public static string PicVideoPath;
        public static string CDesktopPath; // C_DESKTOP folder path
        public static string CDesktopDisplayMode = "0"; // default display mode is 0

        public bool IsHistoryEnabled = true;

        public MainWindow()
        {
            if (LoggedIn == false) new LoginWindow();

            InitializeComponent();
            if (TempPath == null) CheckConfigurationIsRight();

            StartUp();
        }

        #region Time functions
        private void SetTimeLogics()
        {
            clockTime.Content = DateTime.Now.ToString("HH:mm:ss");

            clocktimer = new DispatcherTimer();
            clocktimer.Interval = TimeSpan.FromSeconds(1);
            clocktimer.Tick += UpdateTime;
            clocktimer.Start();
        }
        private void UpdateTime(object sender, EventArgs e)
        {
            clockTime.Content = DateTime.Now.ToString("HH:mm:ss");
        }
        #endregion

        #region ComboBox functions
        private void SetComboBoxPlaceHolder(object sender, RoutedEventArgs e)
        {
            searchComboBox.Text = "Search";
            searchComboBox.Foreground = Brushes.Gray;
            searchComboBox.IsTextSearchEnabled = false;
        }
        private void DeleteComboBoxPlaceHolders(object sender, RoutedEventArgs e)
        {
            searchComboBox.Text = string.Empty;
            searchComboBox.Foreground = RoutineLogics.ConvertHexColor(RoutineLogics.GetFontSettingsFromCfont()[11]);
            searchComboBox.IsTextSearchEnabled = true;
        }
        #endregion

        #region Expander functions
        private void expander_Expanded(object sender, RoutedEventArgs e)
        {
            gridSplitter.Visibility = Visibility.Visible;
            Grid.SetColumnSpan(splitterGrid, 1);

            Grid grid = (Grid)gridSplitter.Parent;
            grid.ColumnDefinitions[2].Width = new GridLength(115);
        }
        private void expander_Collapsed(object sender, RoutedEventArgs e)
        {
            gridSplitter.Visibility = Visibility.Collapsed;
            Grid.SetColumnSpan(splitterGrid, 2);

            Grid grid = (Grid)gridSplitter.Parent;
            grid.ColumnDefinitions[2].Width = new GridLength(23);
        }
        #endregion

        #region OnClosing functions
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (Title == MainItems.MAIN_WIN)
            {
                // Convert ListBox items to an array
                string[] histories = historyList.Items
                    .Cast<ListBoxItem>() 
                    .Select(item => item.Tag.ToString()) // Extract the content as string
                    .ToArray();

                File.WriteAllLines(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), Configs.C_CONFIGS, Configs.CHIST), histories);
                Application.Current.Shutdown();
            }
            clocktimer?.Stop();
            clocktimer = null;
        }
        #endregion

        #region App Clicked functions
        private void NoteApp_Clicked(object sender, RoutedEventArgs e)
        {
            NoteAppLogics.NewNote_Wanted(this, currentDesktop);
        }
        private void MusicAppStart(object sender, RoutedEventArgs e)
        {
            if (RoutineLogics.IsMusicAppOpen() == false)
            {
                new GenMusicApp();
            }
        }
        private void PicMovieApp_Clicked(object sender, RoutedEventArgs e)
        {
            if (RoutineLogics.IsPicMovieOpen() == false)
            {
                new PicMovie
                {
                    window = this,
                    desktopPath = currentDesktop,
                };
            }
        }
        private void GenMailApp_Clicked(object sender, RoutedEventArgs e)
        {
            if (RoutineLogics.IsMailAppOpen() == false)
            {
                new GenMailApp();
            }
        }
        private void GenCalculatorApp_Clicked(object sender, RoutedEventArgs e)
        {
            if (RoutineLogics.IsCalculatorAppOpen() == false)
            {
                new CalculatorApp();
            }
        }
        private void OpenTrashBacket(object sender, RoutedEventArgs e)
        {
            if (RoutineLogics.IsTrashBacketOpen() == false)
            {
                new TrashBacket();
            }
        }
        #endregion

        #region Display Mode functions
        private void DisplayMode_Zero_Checked(object sender, RoutedEventArgs e)
        {
            SaveDisplaySetting("0");
        }
        private void DisplayMode_One_Checked(object sender, RoutedEventArgs e)
        {
            SaveDisplaySetting("1");
        }
        private void SaveDisplaySetting(string mode)
        {
            try
            {
                string CDesktopFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), Configs.C_CONFIGS, Configs.CPATH);

                string[] desktopDisplay = File.ReadAllLines(CDesktopFilePath);
                desktopDisplay[1] = mode;
                File.WriteAllLinesAsync(CDesktopFilePath, desktopDisplay);
                CDesktopDisplayMode = mode;
                RoutineLogics.ReloadNeededForEveryWindow();
            }
            catch (Exception ex)
            {
                RoutineLogics.ErrorMessage("Setting Write Error", "Something went wrong while saving settings.\n", ex.Message);
            }
        }
        #endregion

        #region Configuration functions
        private string ConfigurePath(string CDesktopFile)
        {
            string input = InputDialog.ShowInputDialog("  Please enter " + Configs.CDESK + " path\n\n" +
                "  What is C_DESKTOP?\n\n  C_DESKTOP is a folder that contains\n  your folders, musics, videos etc. and the base of GencOS.\n" +
                "  You have to create one folder named C_DESKTOP regardless of where it is\n  and paste the path of this folder to the given blank in order to continue.\n" +
                "  Other configurations (such as creating C_CONFIGS folder)\n  will be automatically completed.", "Path Needed");
            //Resumes until valid path is entered
            while (true)
            {
                if (InputDialog.Result == true)
                {
                    if (input.EndsWith(Configs.CDESK) == false) input = InputDialog.ShowInputDialog($"Path must end with {Configs.CDESK}.\nCheck if the folder is named correctly.\n(Folder should be named as C_DESKTOP)", "Invalid path", ImagePaths.WRNG_IMG);
                    else if (Directory.Exists(input) == false) input = InputDialog.ShowInputDialog($"Path to {Configs.CDESK} does not exists.\nPlease check if the path is correct.", "Incorrect path", ImagePaths.WRNG_IMG);
                    else
                    {
                        try
                        {
                            string C_CONFIGS = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), Configs.C_CONFIGS);
                            Directory.CreateDirectory(C_CONFIGS); // create the folder that contains all the configuration datas
                            CreateConfigFiles(CDesktopFile, input, C_CONFIGS);
                            CreateHiddenDirectories(input); // create the hidden folders
                            return input;
                        }
                        catch (Exception ex)
                        {
                            RoutineLogics.ErrorMessage(Errors.WRT_ERR, Errors.WRT_ERR_MSG, "the path to file.\n", ex.Message);
                            Environment.Exit(0);
                        }
                    }
                }
                else Environment.Exit(0);
            }
        }
        private bool CheckConfigDirectory(string C_CONFIGS, string CDesktopFile)
        {
            if (Directory.Exists(C_CONFIGS) == false)
            {
                MessageBox.Show($"{Configs.C_CONFIGS} folder not found. Will be created after path is entered.", "Configuration Proccess", MessageBoxButton.OK, MessageBoxImage.Warning);
                TempPath = ConfigurePath(CDesktopFile);
                return true; // configuration proccess will stop
            }
            return false; // configuration proccess will not stop
        }
        private void StartUp()
        {
            try
            {
                currentDesktop = TempPath.Trim();
                string[] CDesktopFilelines = File.ReadAllLines(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), Configs.C_CONFIGS, Configs.CPATH));
                CDesktopPath = CDesktopFilelines[0].Trim();
                CDesktopDisplayMode = CDesktopFilelines[1].Trim();
                MusicAppPath = Path.Combine(CDesktopPath, HiddenFolders.HAUD_FOL);
                PicVideoPath = Path.Combine(CDesktopPath, HiddenFolders.HPV_FOL);
                TrashPath = Path.Combine(CDesktopPath, HiddenFolders.HTRSH_FOL);

                SetTimeLogics();
                SetDisplaySetting();
                SetHistorySettingsButton();
                if (RoutineLogics.historyLoaded == false) RoutineLogics.AddHistoriesFromCHistory(this, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), Configs.C_CONFIGS, Configs.CHIST));

                if (currentDesktop != null) RoutineLogics.ReloadWindow(this, CDesktopDisplayMode);
            }
            catch (Exception ex)
            {
                RoutineLogics.ErrorMessage(Errors.REL_ERR, Errors.REL_ERR_MSG, MainItems.MAIN_WIN, "\n", ex.Message);
            }
        }
        private void CheckConfigurationIsRight()
        {
            string C_CONFIGS = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), Configs.C_CONFIGS);
            string CDesktopFile = Path.Combine(C_CONFIGS, Configs.CPATH);

            if (CheckConfigDirectory(C_CONFIGS, CDesktopFile)) return;

            CheckCDesktopPathFile(CDesktopFile);
            CheckCcolorFile(C_CONFIGS);
            CheckCfontFile(C_CONFIGS);
            CheckCHistoryFile(C_CONFIGS);

            TryReadCDesktopPath(CDesktopFile);

        }
        private void CreateConfigFiles(string CDesktopFile, string input, string C_CONFIGS)
        {
            // C path
            using (StreamWriter writer = new StreamWriter(File.Create(CDesktopFile)))
            {
                writer.WriteLineAsync(input);
                writer.WriteLineAsync('0'); // default display mode
            }
            //C desktop colors
            using (StreamWriter writer = new StreamWriter(Path.Combine(C_CONFIGS, Configs.CCOL)))
            {
                CreateColorConfig(writer);
            }

            // C fonts
            CreateFontConfig(C_CONFIGS);
        }
        private void CreateColorConfig(StreamWriter writer)
        {
            writer.WriteLineAsync(Defaults.MAIN_DESK_COL);
            writer.WriteLineAsync(Defaults.FOL_DESK_COL);
            writer.WriteLineAsync(Defaults.SAFARI_COL);
            writer.WriteLineAsync(Defaults.MENU_COL);
        }
        private void CreateFontConfig(string C_CONFIGS)
        {
            using (StreamWriter writer = new StreamWriter(Path.Combine(C_CONFIGS, Configs.CFONT)))
            {
                // desktop default font
                WriteDefaultFonts(writer);

                // folder default font
                WriteDefaultFonts(writer);

                // menu default font
                WriteDefaultFonts(writer);
            }
        }
        private void WriteDefaultFonts(StreamWriter writer)
        {
            writer.WriteLineAsync(Defaults.FONT);
            writer.WriteLineAsync(Defaults.FONT_COL);
            writer.WriteLineAsync(Defaults.FONT_WEIGHT);
            writer.WriteLineAsync(Defaults.FONT_STYLE);
            writer.WriteLineAsync(Defaults.FONT_SIZE);
        }
        private void CreateHiddenDirectories(string desktopPath)
        {
            string trashpath = Path.Combine(desktopPath, HiddenFolders.HTRSH_FOL);
            string musicpath = Path.Combine(desktopPath, HiddenFolders.HAUD_FOL);
            CreateHiddenFolderOf(trashpath);
            CreateHiddenFolderOf(musicpath);

        }
        private void CheckCDesktopPathFile(string CDesktopFile)
        {
            if (File.Exists(CDesktopFile) == false)
            {
                if (MessageBox.Show($"{Configs.CPATH} could not find. Resetting path needed.", "Path could not find", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                {
                    TempPath = ConfigurePath(CDesktopFile);
                }
                else Environment.Exit(0);
            }
        }
        private void CheckCcolorFile(string C_CONFIGS)
        {
            if (File.Exists(Path.Combine(C_CONFIGS, Configs.CCOL)) == false)
            {
                MessageBox.Show($"{Configs.CCOL} file not found. Creating with default settings", "Creating configuration files", MessageBoxButton.OK, MessageBoxImage.Information);
                using (StreamWriter writer = new StreamWriter(Path.Combine(C_CONFIGS, Configs.CCOL)))
                {
                    CreateColorConfig(writer);
                }
            }
        }
        private void CheckCfontFile(string C_CONFIGS)
        {
            if (File.Exists(Path.Combine(C_CONFIGS, Configs.CFONT)) == false)
            {
                MessageBox.Show($"{Configs.CFONT} file not found. Creating with default settings", "Creating configuration files", MessageBoxButton.OK, MessageBoxImage.Information);
                CreateFontConfig(C_CONFIGS);
            }
        }
        private void CheckCHistoryFile(string C_CONFIGS)
        {
            if (File.Exists(Path.Combine(C_CONFIGS, Configs.CHIST)) == false)
            {
                MessageBox.Show($"{Configs.CHIST} file not found. Creating.", "Creating configuration files", MessageBoxButton.OK, MessageBoxImage.Information);
                File.Create(Path.Combine(C_CONFIGS, Configs.CHIST)).Close();
            }
        }
        private void TryReadCDesktopPath(string CDesktopFile)
        {
            try
            {
                string[] lines = File.ReadAllLines(CDesktopFile);
                TempPath = lines[0];

                if (Directory.Exists(TempPath) == false)
                {
                    if (MessageBox.Show($"Path to {Configs.CDESK} is corrupted or does not exists.\nPlease reset the desktop path", "Incorrect path", MessageBoxButton.OKCancel, MessageBoxImage.Error) == MessageBoxResult.OK)
                    {
                        TempPath = ConfigurePath(CDesktopFile);
                    }
                    else Environment.Exit(0);
                }
                else if (lines.Length < 2)
                {
                    string[] newlines = {TempPath, "0"};
                    File.WriteAllLines(CDesktopFile, newlines);
                }
                else CheckHiddenFolders(TempPath);
            }
            catch (Exception ex)
            {
                RoutineLogics.ErrorMessage(Errors.READ_ERR, Errors.READ_ERR_MSG, "the path from ", Configs.CPATH, "\n", ex.Message);
                Environment.Exit(0);
            }
        }
        private void CheckHiddenFolders(string CDesktopPath)
        {
            CDesktopPath = CDesktopPath.Trim();
            CheckExist(Path.Combine(CDesktopPath, HiddenFolders.HAUD_FOL), HiddenFolders.HAUD_FOL);
            CheckExist(Path.Combine(CDesktopPath, HiddenFolders.HTRSH_FOL), HiddenFolders.HTRSH_FOL);
            CheckExist(Path.Combine(CDesktopPath, HiddenFolders.HPV_FOL), HiddenFolders.HPV_FOL);
        }
        private void CreateHiddenFolderOf(string path)
        {
            Directory.CreateDirectory(path);
            new DirectoryInfo(path) { Attributes = FileAttributes.Hidden };
        }
        private void CheckExist(string path, string foldername)
        {
            if (Directory.Exists(path) == false)
            {
                MessageBox.Show($"{foldername} folder not found. Creating.", "Hidden folder not found", MessageBoxButton.OK, MessageBoxImage.Information);
                CreateHiddenFolderOf(path);
            }
        }
        #endregion

        #region SearchBox menuitem functions
        private void ClearHistory_Wanted(object sender, RoutedEventArgs e)
        {
            historyList.Items.Clear();
        }
        #endregion

        #region History And Display functions
        private void SetHistorySettingsButton()
        {
            enableButton.Checked += (sender, e) =>
            {
                IsHistoryEnabled = true;
                RoutineLogics.MainWindowManuallyReloadNeeded(this);
            };
            disableButton.Checked += (sender, e) =>
            {
                IsHistoryEnabled = false;
                RoutineLogics.MainWindowManuallyReloadNeeded(this);
            };
        }
        private void SetDisplaySetting()
        {
            string mode = File.ReadAllLines(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), Configs.C_CONFIGS, Configs.CPATH))[1].Trim();
            switch (mode)
            {
                case "0":
                    iconMode.Checked -= DisplayMode_Zero_Checked;
                    iconMode.IsChecked = true;
                    iconMode.Checked += DisplayMode_Zero_Checked;
                    break;
                case "1":
                    lineMode.Checked -= DisplayMode_One_Checked;
                    lineMode.IsChecked = true;
                    lineMode.Checked += DisplayMode_One_Checked;
                    break;
            }
        }
        #endregion

        #region Desktop MenuItem Option functions
        private static void ImportFile(MainWindow window, string desktopPath)
        {
            string filter = $"Text Files (*{SupportedFiles.TXT})|*{SupportedFiles.TXT}|RTF Files (*{SupportedFiles.RTF})|*{SupportedFiles.RTF}|WAV Files (*{SupportedFiles.WAV})|*{SupportedFiles.WAV}|MP3 Files (*{SupportedFiles.MP3})|*{SupportedFiles.MP3}|EXE Files (*{SupportedFiles.EXE})|*{SupportedFiles.EXE}|PNG Files (*{SupportedFiles.PNG})|*{SupportedFiles.PNG}|JPG Files (*{SupportedFiles.JPG})|*{SupportedFiles.JPG}";
            RoutineLogics.MoveAnythingWithQuery("Import File", filter, null, desktopPath, desktopPath, 4);

            RoutineLogics.ReloadWindow(window, CDesktopDisplayMode);

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
                        RoutineLogics.ReloadWindow(this, CDesktopDisplayMode);
                    }
                    else RoutineLogics.ErrorMessage(Errors.CRT_ERR, foldername, " already exists.");
                }
            }
            catch (Exception ex)
            {
                RoutineLogics.ErrorMessage(Errors.CRT_ERR, Errors.READ_ERR_MSG, "the folder\n", ex.Message);
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
                    RoutineLogics.WindowReloadNeeded(Path.GetDirectoryName(currentDesktop));
                }
                else
                {
                    RoutineLogics.ErrorMessage(Errors.PRMS_ERR, "You cannot delete ", Configs.CDESK, " folder.");
                }
            }
            catch (Exception ex)
            {
                RoutineLogics.ErrorMessage(Errors.DEL_ERR, Errors.DEL_ERR_MSG, "the folder.\n", ex.Message);
            }
        }
        private void RenameFolderName(object sender, RoutedEventArgs e)
        {
            if (Title != MainItems.MAIN_WIN)
            {
                string newfoldername = InputDialog.ShowInputDialog("Enter the new name:", "Rename Folder", ImagePaths.FOLDER_IMG, ImagePaths.RENM_IMG);
                if (newfoldername == HiddenFolders.HAUD_FOL || newfoldername == HiddenFolders.HTRSH_FOL)
                {
                    RoutineLogics.ErrorMessage(Errors.PRMS_ERR, "You can not rename folders as same as hidden folders.");
                    return;
                }

                if (string.IsNullOrEmpty(newfoldername) == false && newfoldername != Configs.CDESK && newfoldername != MainItems.MAIN_WIN)
                {
                    Title = newfoldername;
                    try
                    {
                        Directory.Move(currentDesktop, Path.Combine(Path.GetDirectoryName(currentDesktop), newfoldername));
                    }
                    catch (Exception ex)
                    {
                        RoutineLogics.ErrorMessage(Errors.MOVE_ERR, Errors.MOVE_ERR_MSG, newfoldername, ex.Message);
                    }
                }
                else RoutineLogics.ErrorMessage(Errors.PRMS_ERR, "You can not rename folders as null, ", Configs.CDESK, " or ", MainItems.MAIN_WIN);
            }
            else RoutineLogics.ErrorMessage(Errors.PRMS_ERR, "You can not rename ", MainItems.MAIN_WIN, " (", Configs.CDESK, ") folder.");

        }
        private void AboutGencosPage_Wanted(object sender, RoutedEventArgs e)
        {
            RoutineLogics.ShowAboutWindow("About GencOS", ImagePaths.HLOGO_IMG, ImagePaths.LOGO_IMG, Versions.GOS_VRS, Messages.ABT_DFLT_MSG);
        }
        private void ReloadWindow_Wanted(object sender, RoutedEventArgs e)
        {
            RoutineLogics.ReloadWindow(this, CDesktopDisplayMode);
        }
        private void ImportFile_Wanted(object sender, RoutedEventArgs e)
        {
            ImportFile(this, currentDesktop);
        }
        private void ColorSettings_Clicked(object sender, RoutedEventArgs e)
        {
            if (RoutineLogics.IsColorSettingsOpen() == false)
            {
                new ColorSettings();
            }
        }
        private void FontSettings_Clicked(object sender, RoutedEventArgs e)
        {
            if (RoutineLogics.IsFontSettingsOpen() == false)
            {
                new FontSettings();
            }
        }
        private void EmptyTrash_Wanted(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure to empty the Trash Backet?", "Empty trash", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    IEnumerable<string> trashes = Directory.EnumerateFileSystemEntries(TrashPath);
                    foreach (string trash in trashes)
                    {
                        File.Delete(trash);
                    }
                    RoutineLogics.WindowReloadNeeded(CDesktopPath);
                }
                catch (Exception ex)
                {
                    RoutineLogics.ErrorMessage(Errors.DEL_ERR, "An error occured while empting Trash Backet\n", ex.Message);
                }
            }
        }
        private void RescueTrashes_Wanted(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure to rescue all files from TrashBacket?", "Empty Trash", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
            {
                IEnumerable<string> trashes = Directory.EnumerateFileSystemEntries(TrashPath);
                foreach (string trash in trashes)
                {
                    try
                    {
                        File.Move(trash, Path.Combine(CDesktopPath, Path.GetFileName(trash)));
                    }
                    catch (Exception ex)
                    {
                        RoutineLogics.ErrorMessage(Errors.REL_ERR, Errors.REL_ERR_MSG, $"Trash Backet, deleting {Path.GetFileName(trash)}", "\n", ex.Message);
                        File.Delete(trash); // delete selected
                    }
                }
                RoutineLogics.WindowReloadNeeded(CDesktopPath);
            }
        }
        private void CalendarApp_Wanted(object sender, RoutedEventArgs e)
        {
            if (RoutineLogics.IsCalendarAppOpen() == false)
            {
                new CalendarApp();
            }
        }
        #endregion

        #region Unclassified private functions
        private void listDesktop_Selected(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem item = (ListBoxItem)listDesktop.SelectedItem;
            if (item == null) return;
            string filepath = item.Tag.ToString();
            switch (Path.GetExtension(item.Tag.ToString()))
            {
                case SupportedFiles.TXT: 
                    RoutineLogics.AddTextListener(this, currentDesktop, filepath);
                    RoutineLogics.AddFileToHistoryListener(this, ImagePaths.TXT_IMG, filepath);
                    break;
                case SupportedFiles.RTF: 
                    RoutineLogics.AddRTFListener(this, currentDesktop, filepath);
                    RoutineLogics.AddFileToHistoryListener(this, ImagePaths.RTF_IMG, filepath);
                    break;
                case SupportedFiles.WAV: 
                    RoutineLogics.AddAudioListener(this, filepath, ImagePaths.WAV_IMG);
                    RoutineLogics.AddFileToHistoryListener(this, ImagePaths.WAV_IMG, filepath);
                    break;
                case SupportedFiles.MP3: 
                    RoutineLogics.AddAudioListener(this, filepath, ImagePaths.MP3_IMG);
                    RoutineLogics.AddFileToHistoryListener(this, ImagePaths.MP3_IMG, filepath);
                    break;
                case SupportedFiles.EXE: 
                    RoutineLogics.AddEXEListener(this, filepath);
                    RoutineLogics.AddFileToHistoryListener(this, ImagePaths.EXE_IMG, filepath);
                    break;
                case SupportedFiles.PNG: 
                    RoutineLogics.AddPictureListener(this, currentDesktop, filepath, ImagePaths.PNG_IMG);
                    RoutineLogics.AddFileToHistoryListener(this, ImagePaths.PNG_IMG, filepath);
                    break;
                case SupportedFiles.JPG: 
                    RoutineLogics.AddPictureListener(this, currentDesktop, filepath, ImagePaths.JPG_IMG);
                    RoutineLogics.AddFileToHistoryListener(this, ImagePaths.JPG_IMG, filepath);
                    break;
                case SupportedFiles.MP4: 
                    RoutineLogics.AddMP4Listener(this, filepath);
                    RoutineLogics.AddFileToHistoryListener(this, ImagePaths.MP4_IMG, filepath);
                    break;
                default: RoutineLogics.AddFolderListener(filepath); break;
                }
            }
        #endregion
    }
}
