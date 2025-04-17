using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

namespace WPFFrameworkApp
{
    /// <summary>
    /// ColorSettings.xaml etkileşim mantığı
    /// </summary>
    public partial class ColorSettings : Window
    {
        public ColorSettings()
        {
            InitializeComponent();
            SetFontPreviewStyles();
            SetStyles();
            Show();
        }

        #region ColorSettings menuitems functions
        private void ApplyChanges_Clicked(object sender, RoutedEventArgs e)
        {
            string[] newcolors = new string[4];
            newcolors[0] = mainDesktopColor.Text;
            newcolors[1] = folderDesktopColor.Text;
            newcolors[2] = safariColor.Text;
            newcolors[3] = menuColor.Text;
            File.WriteAllLines(Path.Combine(RoutineLogics.configFolder, Configs.CCOL), newcolors);
            RoutineLogics.ReloadNeededForEveryWindow();
            Close();
        }
        private void CancelChanges_Clicked(object sender, RoutedEventArgs e)
        {

            if (MessageBox.Show("Are you sure to quit without saving?", "Cancel Changes", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Close();
            }
        }
        private void RestoreDefaults_Wanted(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Do you really want to restore default settings?", "Restore Defaults", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                mainDesktopColor.Text = Defaults.MAIN_DESK_COl;
                folderDesktopColor.Text = Defaults.FOL_DESK_COL;
                safariColor.Text = Defaults.SAFARI_COL;
                menuColor.Text = Defaults.MENU_COL;

                mainDesktopColor.Foreground = Brushes.Black;
                folderDesktopColor.Foreground = Brushes.Black;
                safariColor.Foreground = Brushes.Black;
                menuColor.Foreground = Brushes.Black;
            }
        }
        #endregion

        #region Color change functions
        private void MainDesktopColorChange_Clicked(object sender, RoutedEventArgs e)
        {
            ChangeColorOfText(mainDesktopColor);
        }
        private void FolderDesktopColorChange_Clicked(object sender, RoutedEventArgs e)
        {
            ChangeColorOfText(folderDesktopColor);
        }
        private void SafariColorChange_Clicked(object sender, RoutedEventArgs e)
        {
            ChangeColorOfText(safariColor);
        }
        private void MenuColorChange_Clicked(object sender, RoutedEventArgs e)
        {
            ChangeColorOfText(menuColor);
        }
        private void ChangeColorOfText(TextBlock textblock)
        {
            using (System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog())
            {
                if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    textblock.Foreground = new SolidColorBrush(Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B));
                    int hexcolor = colorDialog.Color.ToArgb(); //X8 means hexadecimal with 8 digits
                    textblock.Text = $"#{hexcolor & 0x00FFFFFF:X6}"; // only RGB, X6 = hexadecimal with 6 digits
                }
            }
        }
        #endregion

        #region Panel Style functions
        private void SetStyles()
        {
            SolidColorBrush menucolor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(RoutineLogics.GetColorSettingsFromCcol()[3]));
            RoutineLogics.SetSettingsForAllMenu(colorMenu, RoutineLogics.GetFontSettingsFromCfont());
            MenuItem[] items = { CItem1, CItem2, CItem3 };

            foreach (MenuItem item in items) item.Background = menucolor;
        }
        private void SetFontPreviewStyles()
        {
            string[] colors = File.ReadAllLines(Path.Combine(RoutineLogics.configFolder, Configs.CCOL));

            mainDesktopColor.Text = colors[0];
            folderDesktopColor.Text = colors[1];
            safariColor.Text = colors[2];
            menuColor.Text = colors[3];
        }
        #endregion
    }
}
