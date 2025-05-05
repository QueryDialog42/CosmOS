using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Win32;
using WPFFrameworkApp2;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace WPFFrameworkApp
{
    public partial class RoutineLogics
    {
        public static string configFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), Configs.C_CONFIGS);
        private static string[] fontcolor = File.ReadAllLines(Path.Combine(configFolder, Configs.CFONT));

        public static string menuFontColor = GetFontColor(fontcolor, 2);
        public static string folderFontcolor = GetFontColor(fontcolor, 1);
        public static string desktopFontcolor = GetFontColor(fontcolor, 0);

        public static bool IsHistoryEnabled = true;

        #region Style functions
        public static void SetWindowStyles(dynamic menu, dynamic[] menuitems)
        {
            string[] fontsettings = GetFontSettingsFromCfont();
            SetSettingsForAllMenu(menu, fontsettings);

            foreach (var item in menuitems)
            {
                SetSettingsForAllMenu(item, fontsettings);
            }
        }
        #endregion

        #region Search functions
        public static void AddEveryItemIntoSearch(MainWindow window)
        {
            window.searchComboBox.Items.Clear(); // clear the search box

            AddAllFoldersIntoSearch(window, window.currentDesktop);
            AddAllAppIntoSearch(window, window.currentDesktop);

            var items = window.searchComboBox.Items.Cast<object>().ToArray();
            SetWindowStyles(window.searchComboBox, items);
        }
        public static void AddAllFoldersIntoSearch(MainWindow window, string folderPath)
        {
            IEnumerable<string> folders = Directory.EnumerateDirectories(folderPath);
            string[] hiddenfolders = { HiddenFolders.HAUD_FOL, HiddenFolders.HTRSH_FOL, HiddenFolders.HPV_FOL };
            foreach (string folder in folders)
            {
                string foldername = Path.GetFileName(folder);
                if (new DirectoryInfo(folder).GetDirectories().Length > 0) AddAllFoldersIntoSearch(window, folder);
                if (hiddenfolders.Contains(foldername) == false)
                {
                    Image imageicon = new Image
                    {
                        Source = new BitmapImage(new Uri(ImagePaths.FOLDER_IMG, UriKind.RelativeOrAbsolute)),
                        VerticalAlignment = VerticalAlignment.Center,
                        Height = 20
                    };

                    TextBlock textblock = new TextBlock
                    {
                        Text = "   " + foldername,
                        VerticalAlignment = VerticalAlignment.Center
                    };

                    StackPanel stackpanel = new StackPanel { Orientation = Orientation.Horizontal };
                    stackpanel.Children.Add(imageicon);
                    stackpanel.Children.Add(textblock);

                    ComboBoxItem comboBoxItem = new ComboBoxItem
                    {
                        Content = stackpanel,
                        Tag = foldername
                    };

                    AddSearchBoxItemListeners(window, comboBoxItem, folder);
                    window?.searchComboBox.Items.Add(comboBoxItem);
                }
            }
        }
        public static void AddAllAppIntoSearch(MainWindow window, string desktopPath)
        {
            IEnumerable<string> files = Directory.EnumerateFileSystemEntries(desktopPath);
            foreach (string file in files)
            {
                if (file.EndsWith(HiddenFolders.HTRSH_FOL)) continue;

                string image;
                string filename = Path.GetFileName(file);
                switch (Path.GetExtension(file))
                {
                    case SupportedFiles.TXT: image = ImagePaths.TXT_IMG; break;
                    case SupportedFiles.RTF: image = ImagePaths.RTF_IMG; break;
                    case SupportedFiles.WAV: image = ImagePaths.WAV_IMG; break;
                    case SupportedFiles.MP3: image = ImagePaths.MP3_IMG; break;
                    case SupportedFiles.EXE: image = ImagePaths.EXE_IMG; break;
                    case SupportedFiles.PNG: image = ImagePaths.PNG_IMG; break;
                    case SupportedFiles.JPG: image = ImagePaths.JPG_IMG; break;
                    case SupportedFiles.MP4: image = ImagePaths.MP4_IMG; break;
                    default: AddAllAppIntoSearch(window, file); continue;
                }

                Image imageicon = new Image
                {
                    Source = new BitmapImage(new Uri(image, UriKind.RelativeOrAbsolute)),
                    VerticalAlignment = VerticalAlignment.Center,
                    Height = 20
                };

                TextBlock textblock = new TextBlock
                {
                    Text = "   " + filename,
                    VerticalAlignment = VerticalAlignment.Center,
                };

                StackPanel stackpanel = new StackPanel { Orientation = Orientation.Horizontal };
                stackpanel.Children.Add(imageicon);
                stackpanel.Children.Add(textblock);

                ComboBoxItem comboBoxItem = new ComboBoxItem
                {
                    Content = stackpanel,
                    Tag = filename
                };

                AddSearchBoxItemListeners(window,comboBoxItem, file);

                window?.searchComboBox.Items.Add(comboBoxItem);
            }
        }
        private static void AddSearchBoxItemListeners(MainWindow window, ComboBoxItem item, string filepath)
        {
            string filename = item.Tag.ToString();
            string desktopPath = Path.GetDirectoryName(filepath);

            item.PreviewMouseLeftButtonUp += (sender, e) =>
            {
                string fileimage = ChooseListenerFor(window, Path.GetDirectoryName(filepath), filepath);
                AddFileToHistoryListener(window, fileimage, filepath);
            };
        }
        #endregion

        #region IsOpen functions
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
        public static bool IsPicMovieOpen()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window is PicMovie)
                {
                    window.Activate();
                    return true;
                }
            }
            return false;
        }
        public static bool IsColorSettingsOpen()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window is ColorSettings)
                {
                    window.Activate();
                    return true;
                }
            }
            return false;
        }
        public static bool IsFontSettingsOpen()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window is FontSettings)
                {
                    window.Activate();
                    return true;
                }
            }
            return false;
        }
        public static bool IsCalendarAppOpen()
        {
            foreach(Window window in Application.Current.Windows)
            {
                if (window is CalendarApp)
                {
                    window.Activate();
                    return true;
                }
            }
            return false;
        }
        public static bool IsCalculatorAppOpen()
        {
            foreach(Window window in Application.Current.Windows)
            {
                if (window is CalculatorApp)
                {
                    window.Activate();
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region ShortKeys functions
        private static ContextMenu SetShortKeyOptions(MainWindow window, string copyicon, string deleteicon, string filepath, string imageicon)
        {
            string[] fontsettings = GetFontSettingsFromCfont();
            string color = GetColorSettingsFromCcol()[3];
            string currentdesktop = Path.GetDirectoryName(filepath);
            string filename = Path.GetFileName(filepath);

            ContextMenu contextMenu = new ContextMenu();
            MenuItem renameItem = CreateMenuItemForContextMenu("Rename", color, ImagePaths.RENM_IMG);
            MenuItem moveitem = CreateMenuItemForContextMenu("Move", color, ImagePaths.NMOVE_IMG);
            MenuItem copyitem = CreateMenuItemForContextMenu("Copy", color, copyicon);
            MenuItem deleteitem = CreateMenuItemForContextMenu("Delete", color, deleteicon);
            MenuItem moreinfo = CreateMenuItemForContextMenu("Details", color, ImagePaths.INFO_IMG);

            renameItem.Click += (sender, e) =>
            {
                RenameFile_Wanted(filepath, imageicon);
                ReloadWindow(window, MainWindow.CDesktopDisplayMode);
            };
            copyitem.Click += (sender, e) =>
            {
                CopyAnythingWithQuery("Copy File", "All Files (*.*)|*.*", filename, currentdesktop, currentdesktop);
                ReloadWindow(window, MainWindow.CDesktopDisplayMode);
            };
            moveitem.Click += (sender, e) =>
            {
                MoveAnythingWithQuery("Move File", "All Files (*.*)|*.*", filename, currentdesktop, currentdesktop, 1);
                ReloadWindow(window, MainWindow.CDesktopDisplayMode);
            };
            deleteitem.Click += (sender, e) =>
            {
                if (MessageBox.Show($"Are you sure to delete {filename}?", "Delete File", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    MoveAnythingWithoutQuery(currentdesktop, filename, Path.Combine(MainWindow.TrashPath, filename));
                    ReloadWindow(window, MainWindow.CDesktopDisplayMode);
                }
            };
            moreinfo.Click += (sender, e) =>
            {
                setInfoWindow(new InfoWindow(), new FileInfo(filepath), imageicon);
            };

            contextMenu.Items.Add(renameItem);
            contextMenu.Items.Add(moveitem);
            contextMenu.Items.Add(copyitem);
            if (imageicon == ImagePaths.WAV_IMG || imageicon == ImagePaths.MP3_IMG) AddIfSound(contextMenu, color, window, currentdesktop, filename);
            else if (imageicon == ImagePaths.PNG_IMG || imageicon == ImagePaths.JPG_IMG) AddIfPicVideo(contextMenu, color, window, currentdesktop, filename, ImagePaths.ADDPIC_IMG);
            else if (imageicon == ImagePaths.MP4_IMG) AddIfPicVideo(contextMenu, color, window, currentdesktop, filename, ImagePaths.ADDMP4_IMG);
            contextMenu.Items.Add(deleteitem);
            contextMenu.Items.Add(moreinfo);

            SetSettingsForAllMenu(contextMenu, fontsettings);

            return contextMenu;
        }
        private static ContextMenu SetShortKeyOptionsForFolders(MainWindow window, string filename)
        {
            string color = GetColorSettingsFromCcol()[3];
            ContextMenu contextMenu = new ContextMenu();
            MenuItem renameItem = CreateMenuItemForContextMenu("Rename", color, ImagePaths.RENM_IMG);
            MenuItem deleteitem = CreateMenuItemForContextMenu("Delete", color, ImagePaths.FDEL_IMG);
            MenuItem folderInfo = CreateMenuItemForContextMenu("Details", color, ImagePaths.INFO_IMG);

            renameItem.Click += (sender, e) => AddRenameFolderListener(window, filename);
            deleteitem.Click += (sender, e) => AddDeleteFolderListener(window, filename);
            folderInfo.Click += (sender, e) => setFolderInfoWindow(Path.Combine(window.currentDesktop, filename));

            contextMenu.Items.Add(renameItem);
            contextMenu.Items.Add(deleteitem);
            contextMenu.Items.Add(folderInfo);

            SetSettingsForAllMenu(contextMenu, GetFontSettingsFromCfont());

            return contextMenu;
        }
        private static MenuItem CreateMenuItemForContextMenu(string header, string color, string icon)
        {
            return new MenuItem
            {
                Header = header,
                Background = ConvertHexColor(color),
                BorderThickness = new Thickness(0),
                Icon = new Image
                {
                    Source = new BitmapImage(new Uri(icon, UriKind.RelativeOrAbsolute)),
                }
            };

        }
        private static string setExtensionString(string extension)
        {
            switch (extension)
            {
                case SupportedFiles.TXT: return $"Text File ({SupportedFiles.TXT})";
                case SupportedFiles.RTF: return $"Rich Text File ({SupportedFiles.RTF})";
                case SupportedFiles.WAV: return $"Waveform Audio File ({SupportedFiles.WAV})";
                case SupportedFiles.MP3: return $"MPEG-1 Audio Layer 3 Audio File ({SupportedFiles.MP3})";
                case SupportedFiles.EXE: return $"Executable File ({SupportedFiles.EXE})";
                case SupportedFiles.PNG: return $"Portable Network Graphics File ({SupportedFiles.PNG})";
                case SupportedFiles.JPG: return $"JPEG Image File ({SupportedFiles.JPG})";
                case SupportedFiles.MP4: return $"MPEG-4 Video File ({SupportedFiles.MP4})";
                default: return "Unknown File (?.?)";
            }
        }
        private static void setInfoWindow(InfoWindow infowindow, FileInfo fileInfo, string imageicon)
        {
            infowindow.nameInfo.Text = fileInfo.Name.Substring(0, fileInfo.Name.Length - 4); // get only name
            infowindow.typeInfo.Text = setExtensionString(fileInfo.Extension);
            infowindow.sizeInfo.Text = $"{fileInfo.Length / 1024} KB";
            infowindow.locInfo.Text = fileInfo.FullName;
            infowindow.LastCreatedInfo.Text = fileInfo.CreationTime.ToString();
            infowindow.LastModifiedInfo.Text = fileInfo.LastWriteTime.ToString();
            infowindow.LastAccessInfo.Text = fileInfo.LastAccessTime.ToString();
            infowindow.SizeToContent = SizeToContent.Width;
            infowindow.logoimage.Source = new BitmapImage(new Uri(imageicon, UriKind.RelativeOrAbsolute));
        }
        private static void setFolderInfoWindow(string directoryPath)
        {

            DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
            InfoWindow infowindow = new InfoWindow { Title = "Directory Details" };

            infowindow.logoimage.Source = new BitmapImage(new Uri(ImagePaths.FOLDER_IMG, UriKind.RelativeOrAbsolute));
            infowindow.nameInfo.Text = directoryInfo.Name;
            infowindow.typeInfo.Text = "Directory";
            infowindow.sizeInfo.Text = $"{directoryInfo.GetFiles().Length} files";
            infowindow.locInfo.Text = directoryInfo.FullName;
            infowindow.LastCreatedInfo.Text = directoryInfo.CreationTime.ToString();
            infowindow.LastModifiedInfo.Text = directoryInfo.LastWriteTime.ToString();
            infowindow.LastAccessInfo.Text = directoryInfo.LastAccessTime.ToString();
            infowindow.SizeToContent = SizeToContent.Width;
        }
        #endregion

        #region GetWindow functions
        public static PicMovie GetPicMovieWindow()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window is PicMovie)
                {
                    return (PicMovie)window;
                }
            }
            return null;
        }
        private static GenMusicApp GetMusicAppWindow()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window is GenMusicApp)
                {
                    return (GenMusicApp)window;
                }
            }
            return null;
        }
        public static MainWindow GetMainWindow(string directoryPath)
        {
            string title;
            if (directoryPath.EndsWith(Configs.CDESK)) title = MainItems.MAIN_WIN;
            else title = GetDirectoryName(directoryPath);
            foreach (Window window in Application.Current.Windows)
            {
                if ((window is MainWindow) && (title == window.Title))
                {
                    return (MainWindow)window;
                }
            }
            return null;
        }
        private static TrashBacket GetTrashBacketWindow()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window is TrashBacket)
                {
                    return (TrashBacket)window;
                }
            }
            return null;
        }
        #endregion

        #region Add History functions
        public static void AddFileToHistoryListener(MainWindow window, string imagepath, string filepath)
        {
            if (IsHistoryEnabled == false) return;

            Image image = new()
            {
                Source = setBitmapImage(imagepath),
                VerticalAlignment = VerticalAlignment.Center,
                Height = 30
            };
            FileInfo fileinfo = new FileInfo(filepath);

            TextBlock filename = new()
            {
                Text = "   " + fileinfo.Name,
                Width = 200,
                VerticalAlignment = VerticalAlignment.Center,
            };
            TextBlock lastAccess = new()
            {
                Text = fileinfo.LastAccessTime.ToString(),
                Width = 200,
                VerticalAlignment = VerticalAlignment.Center
            };
            TextBlock location = new()
            {
                Text = fileinfo.DirectoryName,
                Width = Double.NaN,
                VerticalAlignment = VerticalAlignment.Center
            };

            StackPanel stackpanel = new()
            {
                Orientation = Orientation.Horizontal,
            };

            stackpanel.Children.Add(image);
            stackpanel.Children.Add(filename);
            stackpanel.Children.Add(lastAccess);
            stackpanel.Children.Add(location);

            ListBoxItem historyitem = new()
            {
                Content = stackpanel,
                Tag = filepath
            };

            historyitem.ContextMenu = CreateContextMenuForHistoryItems(window, historyitem);

            historyitem.PreviewMouseLeftButtonUp += (sender, e) => ChooseListenerFor(window, window.currentDesktop, filepath);

            foreach (ListBoxItem item in window.historyList.Items)
            {
                if (item?.Tag?.ToString() == filepath)
                {
                    window?.historyList?.Items?.Remove(item);
                    break;
                }
            }
            window?.historyList?.Items?.Insert(0, historyitem);
        }
        private static ContextMenu CreateContextMenuForHistoryItems(MainWindow window, ListBoxItem historyitem)
        {
            MenuItem deleteitem = new()
            {
                Header = "Delete History",
                Icon = new Image
                {
                    Source = setBitmapImage(ImagePaths.GEN_DEL_IMG)
                }
            };
            deleteitem.Click += (sender, e) => window.historyList.Items.Remove(historyitem);

            ContextMenu contextMenu = new();
            contextMenu.Items.Add(deleteitem); 

            return contextMenu;
        } 
        #endregion

        #region GetSettings functions
        public static string[] GetFontSettingsFromCfont()
        {
            string[] fontsettings = File.ReadAllLines(Path.Combine(configFolder, Configs.CFONT));
            return fontsettings;
        }
        public static string[] GetColorSettingsFromCcol()
        {
            string[] colors = File.ReadAllLines(Path.Combine(configFolder, Configs.CCOL));
            return colors;
        }
        private static string GetFontColor(string[] fontcolor, short which)
        {
            try
            {
                switch (which)
                {
                    case 0: return fontcolor[1]; // desktop font color
                    case 1: return fontcolor[6]; // folder font color
                    case 2: return fontcolor[11]; // menu font color
                    default: return null;
                }
            } catch (IndexOutOfRangeException)
            {
                return null;
                // do nothing
            }
        }
        #endregion

        #region SetSettings functions
        public static void SetSettingsForAllMenu(dynamic menu, string[] fontsettings)
        {
            menu.Background = ConvertHexColor(GetColorSettingsFromCcol()[3]);
            menu.BorderThickness = new Thickness(0);
            menu.FontFamily = new FontFamily(fontsettings[10]);
            menu.Foreground = ConvertHexColor(fontsettings[11]);
            menu.FontWeight = fontsettings[12] == "Bold" ? FontWeights.Bold : FontWeights.Regular;
            menu.FontStyle = fontsettings[13] == "Italic" ? FontStyles.Italic : FontStyles.Normal;
            menu.FontSize = Convert.ToDouble(fontsettings[14]);
        }
        private static void SetWindowSettings(MainWindow window)
        {
            try
            {
                string[] colors = File.ReadAllLines(Path.Combine(configFolder, Configs.CCOL));
                string[] fonts = File.ReadAllLines(Path.Combine(configFolder, Configs.CFONT));

                SetBackgroundSettings(window, colors);
                SetMenuFontSettings(window, fonts);
                SetHistorySettings(window);

                window.clockTime.Foreground = ConvertHexColor(fonts[11]);
            }
            catch (Exception)
            {
                MessageBox.Show("An error occured while configuring desktop. \nDefault settings will be used.", "Configuration Error", MessageBoxButton.OK, MessageBoxImage.Information);

                SetDefaultsForBackgroundColor(window);
                SetDefaultForFonts(window);
                SetSettingsDefault();

                fontcolor[1] = Defaults.FONT_COL;
            }
        }
        private static void SetDefaultsForBackgroundColor(MainWindow window)
        {
            window.desktop.Background = ConvertHexColor(Defaults.MAIN_DESK_COL);
            window.folderdesktop.Background = ConvertHexColor(Defaults.FOL_DESK_COL);
            window.safari.Background = ConvertHexColor(Defaults.SAFARI_COL);
            window.trashApp.Background = ConvertHexColor(Defaults.SAFARI_COL);
            window.menu.Background = ConvertHexColor(Defaults.MENU_COL);
        }
        private static void SetDefaultForFonts(MainWindow window)
        {
            window.FontFamily = new FontFamily(Defaults.FONT);
            window.menu.FontFamily = new FontFamily(Defaults.FONT);
            window.menu.Foreground = ConvertHexColor(Defaults.FONT_COL);
            window.FontWeight = FontWeights.Regular;
            window.FontStyle = FontStyles.Normal;
            window.FontSize = float.Parse(Defaults.FONT_SIZE);
        }
        private static void SetSettingsDefault()
        {
            string[] colors = { Defaults.MAIN_DESK_COL, Defaults.FOL_DESK_COL, Defaults.SAFARI_COL, Defaults.MENU_COL };
            string[] fonts = { Defaults.FONT, Defaults.FONT_COL, Defaults.FONT_WEIGHT, Defaults.FONT_STYLE, Defaults.FONT_SIZE,
                               Defaults.FONT, Defaults.FONT_COL, Defaults.FONT_WEIGHT, Defaults.FONT_STYLE, Defaults.FONT_SIZE,
                               Defaults.FONT, Defaults.FONT_COL, Defaults.FONT_WEIGHT, Defaults.FONT_STYLE, Defaults.FONT_SIZE};

            File.WriteAllLines(Path.Combine(configFolder, Configs.CCOL), colors);
            File.WriteAllLines(Path.Combine(configFolder, Configs.CFONT), fonts);
        }
        private static void SetHistorySettings(MainWindow window)
        {
            if (IsHistoryEnabled)
            {
                Grid.SetRowSpan(window.listDesktop, 1);

                window.historySplitter.Visibility = Visibility.Visible;
                window.historyList.Visibility = Visibility.Visible;
                window.historyMenu.Visibility = Visibility.Visible;
            }
            else
            {
                window.historySplitter.Visibility = Visibility.Collapsed;
                window.historyList.Visibility = Visibility.Collapsed;
                window.historyList.Items.Clear();
                window.historyMenu.Visibility = Visibility.Collapsed;
                window.historyHeight.Height = new GridLength(0);

                Grid.SetRowSpan(window.listDesktop, 2);
            }
        }
        private static void SetApplications(MainWindow window)
        {
            if (window.reloadNeed.IsVisible)
            {
                window.reloadNeed.Visibility = Visibility.Collapsed;
                window.NoteApp.Visibility = Visibility.Visible;
                window.MusicApp.Visibility = Visibility.Visible;
                window.MailApp.Visibility = Visibility.Visible;
                window.PicMovie.Visibility = Visibility.Visible;
                window.CalculatorApp.Visibility = Visibility.Visible;
            }
        }
        private static void SetBackgroundSettings(MainWindow window, string[] colors)
        {
            SolidColorBrush menucolor = ConvertHexColor(colors[3]);
            SolidColorBrush safaricolor = ConvertHexColor(colors[2]);
            SolidColorBrush folderdesktopcolor = ConvertHexColor(colors[1]);
            var maindesktopcolor = ConvertHexColor(colors[0]);

            window.Background = maindesktopcolor;
            window.desktop.Background = Brushes.Transparent;
            window.folderdesktop.Background = folderdesktopcolor;
            window.expander.Background = folderdesktopcolor;
            window.gridSplitter.Background = folderdesktopcolor;
            window.safari.Background = safaricolor;
            window.menu.Background = menucolor;
            window.trashApp.Background = safaricolor;
            window.trashContextMenu.Background = menucolor;
            window.trashContextMenu.BorderThickness = new Thickness(0);

            SetColorOfMenuItems(window, menucolor);
            SetBorderThicknessOfMenuItems(window, new Thickness(0));
        }
        private static void SetMenuFontSettings(MainWindow window, string[] fonts)
        {
            SetSettingsForAllMenu(window.menu, fonts);
            SetSettingsForAllMenu(window.mainContextMenu, fonts);
            SetSettingsForAllMenu(window.trashContextMenu, fonts);
        }
        private static void SetColorOfMenuItems(MainWindow window, SolidColorBrush menucolor)
        {
            MenuItem[] items = {
                window.itemmenu1,
                window.itemmenu2,
                window.itemmenu3,
                window.trashitem1,
                window.trashitem2,
                window.menuitem1,
                window.menuitem2,
                window.menuitem3,
                window.menuitem4,
                window.menuitem5,
                window.menuitem6,
                window.menuitem7,
                window.menuitem8,
                window.menuitem9,
                window.menuitem10,
                window.menuitem11
            };

            foreach (MenuItem item in items)
            {
                item.Background = menucolor;
            }
        }
        private static void SetBorderThicknessOfMenuItems(MainWindow window, Thickness borderthickness)
        {
            MenuItem[] items = {
                window.itemmenu1,
                window.itemmenu2, 
                window.itemmenu3,
                window.trashitem1,
                window.trashitem2, 
                window.menuitem1,
                window.menuitem2,
                window.menuitem3,
                window.menuitem4,
                window.menuitem5,
                window.menuitem6,
                window.menuitem7,
                window.menuitem8,
                window.menuitem9,
                window.menuitem10,
                window.menuitem11
            };

            foreach (MenuItem item in items) 
            {
                item.BorderThickness = borderthickness;
            }
        }
        private static void SetItemFontStylesFor(TextBlock item, string[] fontsettings, byte which)
        {
            switch (which)
            {
                case 0:
                    item.FontFamily = new FontFamily(fontsettings[0]);
                    item.Foreground = ConvertHexColor(fontsettings[1]);
                    item.FontWeight = fontsettings[2] == "Bold" ? FontWeights.Bold : FontWeights.Regular;
                    item.FontStyle = fontsettings[3] == "Italic" ? FontStyles.Italic : FontStyles.Normal;
                    item.FontSize = Convert.ToDouble(fontsettings[4]);
                    break;
                case 1:
                    item.FontFamily = new FontFamily(fontsettings[5]);
                    item.Foreground = ConvertHexColor(fontsettings[6]);
                    item.FontWeight = fontsettings[7] == "Bold" ? FontWeights.Bold : FontWeights.Regular;
                    item.FontStyle = fontsettings[8] == "Italic" ? FontStyles.Italic : FontStyles.Normal;
                    item.FontSize = Convert.ToDouble(fontsettings[9]);
                    break;
            }
        }
        #endregion

        #region Rename file functions
        public static void RenameFile_Wanted(string filepath, string ImagePath, string icon = ImagePaths.RENM_IMG)
        {
            string filename = Path.GetFileName(filepath);
            string currentDesktop = Path.GetDirectoryName(filepath);

            string input = InputDialog.ShowInputDialog("Enter the new name:", "Rename File", ImagePath, icon);

            if (InputDialog.Result == false) return;
            if (CheckInputIsNull(input)) return;

            string newfilename = input.ToLower();
            string pathToDirection = Path.Combine(currentDesktop, newfilename);

            if (CheckNewTXTFilenameIsAllowed(filename, newfilename) == false) return;
            if (CheckNewFilenameIsAllowed(filename, newfilename, SupportedFiles.RTF, "non-RTF") == false) return;
            if (CheckNewFilenameIsAllowed(filename, newfilename, SupportedFiles.WAV, "non-WAV") == false) return;
            if (CheckNewFilenameIsAllowed(filename, newfilename, SupportedFiles.MP3, "non-MP3") == false) return;
            if (CheckNewFilenameIsAllowed(filename, newfilename, SupportedFiles.EXE, "non-EXE") == false) return;
            if (CheckNewFilenameIsAllowed(filename, newfilename, SupportedFiles.PNG, "non-PNG") == false) return;
            if (CheckNewFilenameIsAllowed(filename, newfilename, SupportedFiles.JPG, "non-JPG") == false) return;
            if (CheckNewFilenameIsAllowed(filename, newfilename, SupportedFiles.MP4, "non-MP4") == false) return;

            MoveAnythingWithoutQuery(currentDesktop, filename, pathToDirection); // if this function works, then no error occured
        }
        private static bool CheckNewTXTFilenameIsAllowed(string filename, string newfilename)
        {
            if (filename.EndsWith(SupportedFiles.TXT))
            {
                if (newfilename.EndsWith(SupportedFiles.TXT) == false && newfilename.EndsWith(SupportedFiles.RTF) == false) // TXT to RTF is supported
                {
                    ErrorMessage(Errors.PRMS_ERR, "You can not rename", filename, " as a non-TXT file except RTF file");
                    return false; // not allowed
                }
                return true; // allowed
            }
            return true; // not a txt file
        }
        private static bool CheckNewFilenameIsAllowed(string filename, string newfilename, string checkFor, string non_file)
        {
            if (filename.EndsWith(checkFor))
            {
                if (IsRenameAllowed(filename, newfilename, checkFor) == false)
                {
                    ErrorMessage(Errors.PRMS_ERR, "You can not rename ", filename, $" as a {non_file} file");
                    return false; // not allowed
                }
                return true; // allowed
            }
            return true; // not a rtf file
        }
        private static bool CheckNewFolderNameIsAllowed(string newfoldername)
        {
            if (string.IsNullOrEmpty(newfoldername))
            {
                ErrorMessage(Errors.PRMS_ERR, "You can not rename folders as null"); // not allowed
                return false;
            }
            if (newfoldername == HiddenFolders.HTRSH_FOL || newfoldername == HiddenFolders.HAUD_FOL)
            {
                ErrorMessage(Errors.PRMS_ERR, "You can not rename folders as same as hidden folders.");
                return false; // not allowed
            }
            if (newfoldername == Configs.CDESK || newfoldername == Configs.C_CONFIGS || newfoldername == MainItems.MAIN_WIN)
            {
                ErrorMessage(Errors.PRMS_ERR, "You can not rename folders as ", Configs.CDESK, ", ", Configs.C_CONFIGS, " or ", MainItems.MAIN_WIN);
                return false;
            }
            return true; // allowed
        }
        private static bool IsRenameAllowed(string filename, string newfilename, string checkfor) => filename.EndsWith(checkfor) && newfilename.EndsWith(checkfor);
        #endregion

        #region App Creation functions
        public static Button CreateButton(string filename, byte extrasize = 0)
        {
            return new Button()
            {
                Width = 80 + extrasize,
                Height = 80 + extrasize,
                Background = Brushes.Transparent,
                BorderBrush = Brushes.Transparent,
                ToolTip = filename
            };
        }
        public static Image CreateImage(byte extrasize = 0)
        {
            return new Image
            {
                Width = 60 + extrasize, // Set desired width
                Height = 60 + extrasize, // Set desired height
            };
        }
        public static TextBlock CreateTextBlock(string filename, short which)
        {
            string[] fontsettings = GetFontSettingsFromCfont();

            switch (which)
            {
                case 0: return CreateTextBlockForDesktop(filename, fontsettings);
                case 1: return CreateTextBlockForFolder(filename, fontsettings);
                default: return null;
            }
        }
        public static void Appearence(Image image, StackPanel stackpanel, Button app, TextBlock appname, string logo)
        {
            BitmapImage bitmap = setBitmapImage(logo);
            bitmap.Freeze();
            image.Source = bitmap;

            stackpanel.Children.Add(image);
            stackpanel.Children.Add(appname);

            app.Content = stackpanel;
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
        private static TextBlock CreateTextBlockForDesktop(string filename, string[] fontsettings)
        {
            return new TextBlock()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Text = filename,
                FontFamily = new FontFamily(fontsettings[0]),
                Foreground = ConvertHexColor(fontsettings[1]),
                FontWeight = fontsettings[2] == "Bold" ? FontWeights.Bold : FontWeights.Regular,
                FontStyle = fontsettings[3] == "Italic" ? FontStyles.Italic : FontStyles.Normal,
                FontSize = float.Parse(fontsettings[4])
            };
        }
        private static TextBlock CreateTextBlockForFolder(string filename, string[] fontsettings)
        {
            return new TextBlock()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Text = filename,
                FontFamily = new FontFamily(fontsettings[5]),
                Foreground = ConvertHexColor(fontsettings[6]),
                FontWeight = fontsettings[7] == "Bold" ? FontWeights.Bold : FontWeights.Regular,
                FontStyle = fontsettings[8] == "Italic" ? FontStyles.Italic : FontStyles.Normal,
                FontSize = float.Parse(fontsettings[9])
            };
        }
        private static string ReadTXTFile(string filepath)
        {
            using (StreamReader reader = new StreamReader(filepath))
            {
                StringBuilder stringbuilder = new StringBuilder();
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    stringbuilder.AppendLine(line);
                }
                return stringbuilder.ToString();
            }
        }
        private static void InitTextFile(MainWindow window, string filepath)
        {
            string[] fontsettings = GetFontSettingsFromCfont();
            string filename = Path.GetFileName(filepath);

            Button app = CreateButton(filename);
            TextBlock appname = CreateTextBlock(filename, 0);
            Image image = CreateImage();
            StackPanel stackpanel = new StackPanel { Orientation = Orientation.Vertical };
            Appearence(image, stackpanel, app, appname, ImagePaths.TXT_IMG);

            window.desktop.Children.Add(app);

            app.ContextMenu = SetShortKeyOptions(window, ImagePaths.NCOPY_IMG, ImagePaths.NDEL_IMG, filepath, ImagePaths.TXT_IMG);

            app.Click += (sender, e) =>
            {
                AddTextListener(window, window.currentDesktop, filepath);
                AddFileToHistoryListener(window, ImagePaths.TXT_IMG, filepath);
            };
        }
        private static void InitRTFFile(MainWindow window, string filepath)
        {
            string filename = Path.GetFileName(filepath);
            Button app = CreateButton(filename);
            TextBlock appname = CreateTextBlock(filename, 0);
            Image image = CreateImage();
            StackPanel stackpanel = new StackPanel
            {
                Orientation = Orientation.Vertical
            };
            Appearence(image, stackpanel, app, appname, ImagePaths.RTF_IMG);

            window.desktop.Children.Add(app);

            app.ContextMenu = SetShortKeyOptions(window, ImagePaths.NCOPY_IMG, ImagePaths.NDEL_IMG, Path.Combine(window.currentDesktop, filename), ImagePaths.RTF_IMG);

            app.Click += (sender, e) =>
            {
                AddRTFListener(window, window.currentDesktop, filepath);
                AddFileToHistoryListener(window, ImagePaths.RTF_IMG, filepath);
            };
        }
        private static void InitAudioFile(MainWindow window, string filepath, string fileimage)
        {
            string filename = Path.GetFileName(filepath);
            Button app = CreateButton(filename);
            TextBlock appname = CreateTextBlock(filename, 0);
            Image image = CreateImage();
            StackPanel stackpanel = new StackPanel
            {
                Orientation = Orientation.Vertical
            };

            Appearence(image, stackpanel, app, appname, fileimage);

            window.desktop.Children.Add(app);

            app.ContextMenu = SetShortKeyOptions(window, ImagePaths.SCOPY_IMG, ImagePaths.SDEL_IMG, filepath, fileimage);

            app.Click += (o, e) =>
            {
                AddAudioListener(window, filepath, fileimage);
                AddFileToHistoryListener(window, fileimage, filepath);
            };
        }
        private static void InitEXEFile(MainWindow window, string filepath)
        {
            string filename = Path.GetFileName(filepath);

            Button app = CreateButton(filename);
            TextBlock appname = CreateTextBlock(filename, 0);
            Image image = CreateImage();
            StackPanel stackpanel = new StackPanel
            {
                Orientation = Orientation.Vertical
            };
            Appearence(image, stackpanel, app, appname, ImagePaths.EXE_IMG);

            window.desktop.Children.Add(app);

            app.ContextMenu = SetShortKeyOptions(window, ImagePaths.EXE_IMG, ImagePaths.DELEXE_IMG, filepath, ImagePaths.EXE_IMG);

            app.Click += (s, e) =>
            {
                AddEXEListener(window, filepath);
                AddFileToHistoryListener(window, ImagePaths.EXE_IMG, filepath);
            };
        }
        private static void InitPictureFile(MainWindow window, string filepath, string fileimage)
        {
            string filename = Path.GetFileName(filepath);

            Button app = CreateButton(filename);
            TextBlock appname = CreateTextBlock(filename, 0);
            Image image = CreateImage();
            StackPanel stackpanel = new StackPanel
            {
                Orientation = Orientation.Vertical
            };
            Appearence(image, stackpanel, app, appname, fileimage);

            window.desktop.Children.Add(app);

            app.ContextMenu = SetShortKeyOptions(window, ImagePaths.COPYPIC, ImagePaths.DELPNG_IMG, filepath, fileimage);

            app.Click += (s, e) =>
            {
                AddPictureListener(window, window.currentDesktop, filepath, fileimage);
                AddFileToHistoryListener(window, fileimage, filepath);
            };
        }
        private static void InitMP4File(MainWindow window, string filepath)
        {
            string filename = Path.GetFileName(filepath);
            
            Button app = CreateButton(filename);
            TextBlock appname = CreateTextBlock(filename, 0);
            Image image = CreateImage();
            StackPanel stackpanel = new StackPanel
            {
                Orientation = Orientation.Vertical
            };
            Appearence(image, stackpanel, app, appname, ImagePaths.MP4_IMG);
            window.desktop.Children.Add(app);

            app.ContextMenu = SetShortKeyOptions(window, ImagePaths.COPYMP4_IMG, ImagePaths.DELMP4_IMG, filepath, ImagePaths.MP4_IMG);

            app.Click += (sender, e) =>
            {
                AddMP4Listener(window, filepath);
                AddFileToHistoryListener(window, ImagePaths.MP4_IMG, filepath);
            };
        }
        public static BitmapImage setBitmapImage(string imagepath)
        {
            try
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad; // Load the image into memory
                bitmap.UriSource = new Uri(imagepath, UriKind.RelativeOrAbsolute);
                bitmap.EndInit();
                bitmap.Freeze(); // Make the image thread-safe

                return bitmap;
            } catch (Exception ex)
            {
                ErrorMessage("Unknown Image", "Un located file or image, it may be deleted:\n", ex.Message);
                return null;
            }
            
        }
        public static PicWindow OpenPicWindow(MainWindow window, string desktopPath, string filename)
        {
            BitmapImage icon = new BitmapImage(new Uri(filename.EndsWith(SupportedFiles.PNG) ? ImagePaths.PNG_IMG : ImagePaths.JPG_IMG, UriKind.RelativeOrAbsolute));
            icon.Freeze();

            PicWindow pictureApp = new PicWindow
            {
                Title = filename,
                Icon = icon,
                window = window,
                desktopPath = desktopPath,
                MaxHeight = SystemParameters.PrimaryScreenHeight * 0.8, // %80 of main screen height
                MaxWidth = SystemParameters.PrimaryScreenWidth * 0.9, // %90 of main screen width
                MinHeight = 200,
                MinWidth = 200,
            };

            return pictureApp;
        }
        private static void InitFolder(MainWindow window, string filepath)
        {
            string filename = Path.GetFileName(filepath);
            Button app = CreateButton(filename);
            TextBlock appname = CreateTextBlock(filename, 1);
            Image image = CreateImage();
            StackPanel stackpanel = new StackPanel
            {
                Orientation = Orientation.Vertical
            };
            Appearence(image, stackpanel, app, appname, ImagePaths.FOLDER_IMG);

            window.folderdesktop.Children.Add(app);

            app.ContextMenu = SetShortKeyOptionsForFolders(window, filename);

            app.Click += (sender, e) => AddFolderListener(filepath);
        }

        #endregion

        #region App Listener functions
        public static void AddTextListener(MainWindow window, string desktopPath, string filepath)
        {
            string filename = Path.GetFileName(filepath);
            try
            {
                TXTNote noteapp = new TXTNote
                {
                    windowForNote = window,
                    currentDesktopForNote = desktopPath,
                    Title = filename
                };

                noteapp.note.Text = ReadTXTFile(filepath);
            }
            catch (Exception ex)
            {
                ErrorMessage("TXT" + Errors.READ_ERR, Errors.READ_ERR_MSG, filename ?? "null File", "\n", ex.Message);
            }

            
        }
        public static void AddFolderListener(string filepath)
        {
            MainWindow.TempPath = filepath;
            MainWindow newWindow = new MainWindow
            {
                Title = Path.GetFileName(filepath),
            };
        }
        public static void AddRTFListener(MainWindow window, string desktopPath, string filepath)
        {
            string filename = Path.GetFileName(filepath);
            RTFNote noteapp = new RTFNote
            {
                windowForNote = window,
                currentDesktopForNote = desktopPath,
                Title = filename
            };
            try
            {
                using (StreamReader reader = new StreamReader(filepath))
                {
                    StringBuilder stringbuilder = new StringBuilder();
                    string line;
                    while ((line = reader.ReadLine()) != null)
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
        }
        public static void AddAudioListener(MainWindow window, string filepath, string fileimage)
        {
            string filename = Path.GetFileName(filepath);
            CloseAllGenMusicApps();
            GenMusicApp.mediaPlayer?.Close();
            GenMusicApp.mediaPlayer = null;
            GenMusicApp.isPaused = true;
            GenMusicApp musicapp = new GenMusicApp();
            musicapp.MusicAppButton_Clicked(filepath, filename);
        }
        public static void AddEXEListener(MainWindow window, string filepath)
        {
            string filename = Path.GetFileName(filepath);
            string[] options = { "Run", "Cancel" };
            if (QueryDialog.ShowQueryDialog($"Are you sure you want to run {filename}?", "Executable File Run", options, ImagePaths.EXE_IMG) == 0)
            {
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
                    ErrorMessage(Errors.RUN_ERR, Errors.RUN_ERR_MSG, filename ?? "null File", "\n", ex.Message);
                }
            }
        }
        public static void AddPictureListener(MainWindow window, string desktopPath, string filepath, string fileimage)
        {
            PicWindow pictureApp = OpenPicWindow(window, desktopPath, Path.GetFileName(filepath));

            pictureApp.PicMain.Source = setBitmapImage(filepath);
            pictureApp.SizeToContent = SizeToContent.WidthAndHeight;
        }
        public static void AddMP4Listener(MainWindow window, string filepath)
        {
            var videoApp = new VideoWindow
            {
                Title = Path.GetFileName(filepath),
                desktopPath = Path.GetDirectoryName(filepath)
            };
            videoApp.videoPlayer.Source = new Uri(filepath);
        }
        public static string ChooseListenerFor(MainWindow window, string desktopPath, string filepath)
        {
            switch (Path.GetExtension(filepath))
            {
               case SupportedFiles.TXT: AddTextListener(window, desktopPath, filepath); return ImagePaths.TXT_IMG;
               case SupportedFiles.RTF: AddRTFListener(window, desktopPath, filepath); return ImagePaths.RTF_IMG;
               case SupportedFiles.WAV: AddAudioListener(window, filepath, ImagePaths.WAV_IMG); return ImagePaths.WAV_IMG;
               case SupportedFiles.MP3: AddAudioListener(window, filepath, ImagePaths.MP3_IMG); return ImagePaths.MP3_IMG;
               case SupportedFiles.EXE: AddEXEListener(window,filepath); return ImagePaths.EXE_IMG;
               case SupportedFiles.PNG: AddPictureListener(window, desktopPath, filepath, ImagePaths.PNG_IMG); return ImagePaths.PNG_IMG;
               case SupportedFiles.JPG: AddPictureListener(window, desktopPath, filepath, ImagePaths.JPG_IMG); return ImagePaths.JPG_IMG;
               case SupportedFiles.MP4: AddMP4Listener(window, filepath); return ImagePaths.MP4_IMG;
               default: AddFolderListener(filepath); return ImagePaths.FOLDER_IMG;
            }
        }
        #endregion

        #region Window Reload functions
        public static void ReloadWindow(MainWindow window, string displayMode)
        {
            if (window == null) return; // If window is null, then no reload needed
            if (window.Title != MainItems.MAIN_WIN) IsHistoryEnabled = false;

            switch (displayMode)
            {
                case "0": ReloadForDisplayMode_Zero(window); break;
                case "1": ReloadForDisplayMode_One(window); break;
            }
        }
        private static void ReloadForDisplayMode_Zero(MainWindow window)
        {
            PrepareForReload(window, "0");

            window.desktop.Children.Clear();
            window.folderdesktop.Children.Clear();
            SetWindowSettings(window);
            try
            {
                string[] hiddenfolders = { HiddenFolders.HAUD_FOL, HiddenFolders.HTRSH_FOL, HiddenFolders.HPV_FOL };
                IEnumerable<string> files = Directory.EnumerateFileSystemEntries(window.currentDesktop);
                foreach (string filepath in files)
                {
                    string filename = Path.GetFileName(filepath);
                    if (Directory.Exists(filepath))
                    {
                        if (hiddenfolders.Contains(filename) == false) InitFolder(window, filepath);
                        else // then it is trashbacket
                        {
                            Grid.SetColumnSpan(window.safari, 1);
                            window.trashApp.Visibility = Visibility.Visible;
                            window.trashimage.Source = InitTrashBacket();
                        }
                        continue;
                    }
                    switch (Path.GetExtension(filename))
                    {
                        case SupportedFiles.TXT: InitTextFile(window, filepath); break;
                        case SupportedFiles.RTF: InitRTFFile(window, filepath); break;
                        case SupportedFiles.WAV: InitAudioFile(window, filepath, ImagePaths.WAV_IMG); break;
                        case SupportedFiles.MP3: InitAudioFile(window, filepath, ImagePaths.MP3_IMG); break;
                        case SupportedFiles.EXE: InitEXEFile(window, filepath); break;
                        case SupportedFiles.PNG: InitPictureFile(window, filepath, ImagePaths.PNG_IMG); break;
                        case SupportedFiles.JPG: InitPictureFile(window, filepath, ImagePaths.JPG_IMG); break;
                        case SupportedFiles.MP4: InitMP4File(window, filepath); break;
                        default: ErrorMessage(Errors.UNSUPP_ERR, filename, " is not supported for ", Versions.GOS_VRS); File.Delete(filepath); break;
                    }
                }
                if (window.searchComboBox != null) AddEveryItemIntoSearch(window);
                SetApplications(window);
                window.Show();
            }
            catch (Exception e)
            {
                ErrorMessage(Errors.REL_ERR, Errors.REL_ERR_MSG, "Main Window\n", e.Message);
                MainWindowManuallyReloadNeeded(window);
            }
        }
        private static void ReloadForDisplayMode_One(MainWindow window)
        {
            PrepareForReload(window, "1");

            window.listDesktop.Items.Clear();
            SetWindowSettings(window);
            try
            {
                string[] hiddenfolders = { HiddenFolders.HAUD_FOL, HiddenFolders.HTRSH_FOL, HiddenFolders.HPV_FOL };
                IEnumerable<string> folders = Directory.EnumerateDirectories(window.currentDesktop);
                foreach (var folderpath in folders)
                {
                    string foldername = Path.GetFileName(folderpath);
                    if (foldername == hiddenfolders[1])
                    {
                        Grid.SetColumnSpan(window.safari, 1);
                        window.trashApp.Visibility = Visibility.Visible;
                        window.trashimage.Source = InitTrashBacket();
                        continue;
                    }
                    else if (hiddenfolders.Contains(foldername)) continue;

                    ReturnNewFolderItem(window, folderpath);
                }

                IEnumerable<string> files = Directory.EnumerateFiles(window.currentDesktop);
                foreach (string filepath in files)
                {
                    switch (Path.GetExtension(filepath))
                    {
                        case SupportedFiles.TXT: CreateTextItem(window, filepath); break;
                        case SupportedFiles.RTF: CreateRTFItem(window, filepath); break;
                        case SupportedFiles.WAV: CreateWAVItem(window, filepath); break;
                        case SupportedFiles.MP3: CreateMP3Item(window, filepath); break;
                        case SupportedFiles.EXE: CreateEXEItem(window, filepath); break;
                        case SupportedFiles.PNG: CreatePNGItem(window, filepath); break;
                        case SupportedFiles.JPG: CreateJPGItem(window, filepath); break;
                        case SupportedFiles.MP4: CreateMP4Item(window, filepath); break;
                        default: ErrorMessage(Errors.UNSUPP_ERR, Path.GetFileName(filepath), " is not supported for ", Versions.GOS_VRS); File.Delete(filepath); break;
                    }
                }
                if (window.searchComboBox != null) AddEveryItemIntoSearch(window);
                SetApplications(window);
                window.Show();
            }
            catch (Exception e)
            {
                ErrorMessage(Errors.REL_ERR, Errors.REL_ERR_MSG, "Main Window\n", e.Message);
                MainWindowManuallyReloadNeeded(window);
            }
        }
        public static void WindowReloadNeeded(string directoryName)
        {
            MainWindow directionFolder = GetMainWindow(directoryName);
            if (directionFolder != null && directoryName != null) ReloadWindow(directionFolder, MainWindow.CDesktopDisplayMode);
        }
        public static void ReloadNeededForEveryWindow()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window is MainWindow)
                {
                    MainWindowManuallyReloadNeeded((MainWindow)window);
                }
            }
        }
        public static void MainWindowManuallyReloadNeeded(MainWindow mainfolder)
        {
            mainfolder.reloadNeed.Visibility = Visibility.Visible;
            mainfolder.NoteApp.Visibility = Visibility.Hidden;
            mainfolder.MusicApp.Visibility = Visibility.Collapsed;
            mainfolder.MailApp.Visibility = Visibility.Collapsed;
            mainfolder.PicMovie.Visibility = Visibility.Collapsed;
            mainfolder.CalculatorApp.Visibility = Visibility.Collapsed;
        }
        private static void MusicAppReloadNeeded()
        {
            GenMusicApp musicapp = GetMusicAppWindow();
            if (musicapp != null)
            {
                musicapp.IsReloadNeeded(true);
            }
        }
        private static void TrashBacketReloadNeeded()
        {
            TrashBacket trashapp = GetTrashBacketWindow();
            trashapp?.ReloadWindow();
        }
        #endregion

        #region File Movement functions
        public static void MoveAnythingWithQuery(string title, string filter, string selectedFileName, string initialDirectory, string currentDesktop, byte toWhere)
        {
            switch (toWhere)
            {
                case 1:
                    MoveSomeWhere(title, filter, selectedFileName, initialDirectory, currentDesktop);
                    break;
                case 2:
                    MoveCertainWindow(title, filter, initialDirectory, currentDesktop, MainWindow.MusicAppPath);
                    MusicAppReloadNeeded();
                    break;
                case 3:
                    MoveCertainWindow(title, filter, initialDirectory, currentDesktop, MainWindow.TrashPath);
                    TrashBacketReloadNeeded();
                    break;
                case 4:
                    MoveCertainWindow(title, filter, initialDirectory, currentDesktop, MainWindow.CDesktopPath);
                    WindowReloadNeeded(MainWindow.CDesktopPath);
                    break;
                case 5:
                    MoveCertainWindow(title, filter, initialDirectory, currentDesktop, MainWindow.PicVideoPath);
                    break;
            }
        }
        public static void MoveAnythingWithoutQuery(string currentDesktop, string filename, string pathToDirection) // path to the file will go
        {
            try
            {
                File.Move(Path.Combine(currentDesktop, filename), pathToDirection);
                string directoryName = Path.GetDirectoryName(pathToDirection);
                if (directoryName == MainWindow.TrashPath) TrashBacketReloadNeeded();
                else if (directoryName == MainWindow.MusicAppPath) MusicAppReloadNeeded();
                else WindowReloadNeeded(directoryName);
            }
            catch (Exception e)
            {
                ErrorMessage(Errors.MOVE_ERR, Errors.MOVE_ERR_MSG, filename ?? "null File", "\n", e.Message);
            }
        }
        public static void CopyAnythingWithQuery(string title, string filter, string selectedFileName, string initialDirectory, string currentDesktop)
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
                if (CheckIfDirectionNotHidden(filepath))
                {
                    try
                    {
                        File.Copy(Path.Combine(currentDesktop, filename), filepath);
                        WindowReloadNeeded(Path.GetDirectoryName(filepath));
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
        private static void MoveSomeWhere(string title, string filter, string selectedFileName, string initialDirectory, string currentDesktop)
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
                if (CheckIfDirectionNotHidden(filepath))
                {
                    try
                    {
                        File.Move(Path.Combine(currentDesktop, filename), filepath);
                        WindowReloadNeeded(Path.GetDirectoryName(filepath));
                    }
                    catch (Exception ex)
                    {
                        ErrorMessage(Errors.MOVE_ERR, Errors.MOVE_ERR_MSG, filename ?? "null File", "\n", ex.Message);
                    }
                }
                else // user tried to move into hidden folders
                {
                    ErrorMessage(Errors.PRMS_ERR, "You can not move or copy files into hidden folders manually");
                }
            }
        }
        private static void MoveCertainWindow(string title, string filter, string initialDirectory, string currentDesktop, string windowPath)
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
                        File.Move(filepath, Path.Combine(windowPath, filename));
                    }
                    catch (Exception ex)
                    {
                        ErrorMessage(Errors.MOVE_ERR, Errors.MOVE_ERR_MSG, filename ?? "null File", "\n", ex.Message);
                    }
                }
            }
        }
        #endregion

        #region Item App Creation functions
        private static void CreateTextItem(MainWindow window, string filepath)
        {
            CompleteItem(window, filepath, ImagePaths.NCOPY_IMG, ImagePaths.NDEL_IMG, ImagePaths.TXT_IMG);
        }
        private static void CreateRTFItem(MainWindow window, string filepath)
        {
            CompleteItem(window, filepath, ImagePaths.NCOPY_IMG, ImagePaths.NDEL_IMG, ImagePaths.RTF_IMG);
        }
        private static void CreateWAVItem(MainWindow window, string filepath)
        {
            CompleteItem(window, filepath, ImagePaths.SCOPY_IMG, ImagePaths.SDEL_IMG, ImagePaths.WAV_IMG);
        }
        private static void CreateMP3Item(MainWindow window, string filepath)
        {
            CompleteItem(window, filepath, ImagePaths.SCOPY_IMG, ImagePaths.SDEL_IMG, ImagePaths.MP3_IMG);
        }
        private static void CreateEXEItem(MainWindow window, string filepath)
        {
            CompleteItem(window, filepath, ImagePaths.EXE_IMG, ImagePaths.DELEXE_IMG, ImagePaths.EXE_IMG);
        }
        private static void CreatePNGItem(MainWindow window, string filepath)
        {
            CompleteItem(window, filepath, ImagePaths.COPYPIC, ImagePaths.DELPNG_IMG, ImagePaths.PNG_IMG);
        }
        private static void CreateJPGItem(MainWindow window, string filepath)
        {
            CompleteItem(window, filepath, ImagePaths.COPYPIC, ImagePaths.DELPNG_IMG, ImagePaths.JPG_IMG);
        }
        private static void CreateMP4Item(MainWindow window, string filepath)
        {
            CompleteItem(window, filepath, ImagePaths.COPYMP4_IMG, ImagePaths.DELMP4_IMG, ImagePaths.MP4_IMG);
        }
        private static ListBoxItem ReturnNewFileItem(string filepath, string imagepath)
        {
            var fileinfo = new FileInfo(filepath);

            var image = new Image
            {
                Source = setBitmapImage(imagepath),
                VerticalAlignment = VerticalAlignment.Center,
                Height = 30
            };

            var filename = new TextBlock
            {
                Text = "   " + fileinfo.Name,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 180
            };

            var filelength = new TextBlock
            {
                Text = $"{fileinfo.Length / 1024} KB",
                VerticalAlignment = VerticalAlignment.Center,
                Width = 150
            };

            var lastModified = new TextBlock
            {
                Text = fileinfo.LastWriteTime.ToString(),
                VerticalAlignment = VerticalAlignment.Center,
                Width = 150
            };

            string[] fontsettings = GetFontSettingsFromCfont();
            SetItemFontStylesFor(filename, fontsettings, 0);
            SetItemFontStylesFor(filelength, fontsettings, 0);
            SetItemFontStylesFor(lastModified, fontsettings, 0);

            var stackpanel = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };

            stackpanel.Children.Add(image);
            stackpanel.Children.Add(filename);
            stackpanel.Children.Add(filelength);
            stackpanel.Children.Add(lastModified);

            ListBoxItem fileitem = new ListBoxItem
            {
                Content = stackpanel,
                Tag = filepath
            };
            return fileitem;
        }
        private static void ReturnNewFolderItem(MainWindow window, string filepath)
        {
            var folderinfo = new DirectoryInfo(filepath);

            var image = new Image
            {
                Source = setBitmapImage(ImagePaths.FOLDER_IMG),
                VerticalAlignment = VerticalAlignment.Center,
                Height = 30
            };

            var foldername = new TextBlock
            {
                Text = "  " + folderinfo.Name,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 180
            };

            var whenCreated = new TextBlock
            {
                Text = folderinfo.CreationTimeUtc.ToString(),
                VerticalAlignment = VerticalAlignment.Center,
                Width = 150
            };

            var lastAcces = new TextBlock
            {
                Text = folderinfo.LastAccessTimeUtc.ToString(),
                VerticalAlignment = VerticalAlignment.Center,
                Width = 150
            };

            string[] fontsettings = GetFontSettingsFromCfont();
            SetItemFontStylesFor(foldername, fontsettings, 1);
            SetItemFontStylesFor(whenCreated, fontsettings, 1);
            SetItemFontStylesFor(lastAcces, fontsettings, 1);

            var stackpanel = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };

            stackpanel.Children.Add(image);
            stackpanel.Children.Add(foldername);
            stackpanel.Children.Add(whenCreated);
            stackpanel.Children.Add(lastAcces);

            ListBoxItem folderitem = new ListBoxItem
            {
                BorderThickness = new Thickness(0),
                Background = ConvertHexColor(GetColorSettingsFromCcol()[1]),
                Content = stackpanel,
                Tag = filepath
            };

            folderitem.ContextMenu = SetShortKeyOptionsForFolders(window, Path.GetFileName(filepath));
            window.listDesktop.Items.Add(folderitem);
        }
        private static void CompleteItem(MainWindow window, string filepath, string copyicon, string deleteicon, string imageicon)
        {
            var fileitem = ReturnNewFileItem(filepath, imageicon);

            fileitem.ContextMenu = SetShortKeyOptions(window, copyicon, deleteicon, filepath, imageicon);
            window.listDesktop.Items.Add(fileitem);
        }
        #endregion

        #region Unclassified public functions
        public static void ErrorMessage(string errtitle, params string[] errmessage)
        {
            StringBuilder stringbuilder = new StringBuilder();
            foreach (string str in errmessage)
            {
                stringbuilder.Append(str);
            }
            MessageBox.Show(stringbuilder.ToString(), errtitle, MessageBoxButton.OK, MessageBoxImage.Error);
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
        public static string TimeFormat(double totaltime)
        {
            int minutes = (int)(totaltime / 60);
            int seconds = (int)(totaltime % 60);

            return string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        public static SolidColorBrush ConvertHexColor(string hexcolor)
        {
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString(hexcolor));
        }
        #endregion

        #region Unclassified private functions
        private static string GetDirectoryName(string directoryPath)
        {
            string[] parts = directoryPath.Split(Path.DirectorySeparatorChar);
            if (parts.Length > 0)
            {
                return parts[parts.Length - 1];
            }
            return string.Empty;
        }
        private static bool CheckInputIsNull(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                ErrorMessage(Errors.PRMS_ERR, "You can not rename your file as null");
                return true;
            }
            return false;
        }
        private static bool CheckIfDirectionNotHidden(string filepath)
        {
            string[] hiddenfolders = { HiddenFolders.HAUD_FOL, HiddenFolders.HTRSH_FOL, HiddenFolders.HPV_FOL };
            if (hiddenfolders.Contains(Path.GetFileName(Path.GetDirectoryName(filepath)))) return false;
            else return true;
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
        private static void AddIfSound(ContextMenu contextMenu, string color, MainWindow window, string currentdesktop, string filename)
        {
            MenuItem addtogenmuic = CreateMenuItemForContextMenu("Add to GenMusic", color, ImagePaths.SADD_IMG);

            addtogenmuic.Click += (sender, e) =>
            {
                MoveAnythingWithoutQuery(currentdesktop, filename, Path.Combine(MainWindow.MusicAppPath, filename));
                ReloadWindow(window, MainWindow.CDesktopDisplayMode);
                GenMusicApp genmusicapp = GetMusicAppWindow();
                genmusicapp?.IsReloadNeeded(true);
            };

            contextMenu.Items.Add(addtogenmuic);
        }
        private static void AddIfPicVideo(ContextMenu contextMenu, string color, MainWindow window, string currentdesktop, string filename, string addfilemenuicon)
        {
            MenuItem addtopicmovie = CreateMenuItemForContextMenu("Add to PicMovie", color, addfilemenuicon);

            addtopicmovie.Click += (sender, e) =>
            {
                MoveAnythingWithoutQuery(currentdesktop, filename, Path.Combine(MainWindow.PicVideoPath, filename));
                ReloadWindow(window, MainWindow.CDesktopDisplayMode);
                PicMovie picmovie = GetPicMovieWindow();
                picmovie?.ReloadWindow();
            };

            contextMenu.Items.Add(addtopicmovie);
        }
        private static void AddRenameFolderListener(MainWindow window, string filename)
        {
            string newfilename = InputDialog.ShowInputDialog("Enter the new name:", "Rename Folder", ImagePaths.FOLDER_IMG, ImagePaths.RENM_IMG);
            if (InputDialog.Result == false) return;
            if (CheckNewFolderNameIsAllowed(newfilename))
            {
                try
                {
                    Directory.Move(Path.Combine(window.currentDesktop, filename), Path.Combine(window.currentDesktop, newfilename));
                    ReloadWindow(window, MainWindow.CDesktopDisplayMode);
                }
                catch (Exception ex)
                {
                    ErrorMessage(Errors.MOVE_ERR, Errors.MOVE_ERR_MSG, filename ?? "null Folder", "\n", ex.Message);
                }
            }
        }
        private static void AddDeleteFolderListener(MainWindow window, string filename)
        {
            if (MessageBox.Show($"Are you sure to delete {filename} folder?", "Folder Delete", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    Directory.Delete(Path.Combine(window.currentDesktop, filename));
                    ReloadWindow(window, MainWindow.CDesktopDisplayMode);
                }
                catch (Exception ex)
                {
                    ErrorMessage(Errors.DEL_ERR, Errors.DEL_ERR_MSG, filename ?? "null Folder", "\n", ex.Message);
                }
            }
        }
        private static void PrepareForReload(MainWindow window, string mode)
        {
            if (mode == "0")
            {
                window?.listDesktop?.Items.Clear();
                window.listDesktop.Visibility = Visibility.Collapsed;

                window.desktop.Visibility = Visibility.Visible;
                window.folderdesktop.Visibility = Visibility.Visible;
                window.expander.Visibility = Visibility.Visible;

                Grid grid = (Grid)window.gridSplitter.Parent;
                grid.ColumnDefinitions[2].Width = GridLength.Auto;
            }
            else if (mode == "1")
            {
                window.desktop?.Children.Clear();
                window.folderdesktop?.Children.Clear();
                window.desktop.Visibility= Visibility.Collapsed;
                window.folderdesktop.Visibility= Visibility.Collapsed;
                window.expander.Visibility = Visibility.Collapsed;

                Grid grid = (Grid)window.gridSplitter.Parent;
                grid.ColumnDefinitions[2].Width = new GridLength(0);

                window.listDesktop.Visibility= Visibility.Visible;
            }
        }
        #endregion
    }
}
