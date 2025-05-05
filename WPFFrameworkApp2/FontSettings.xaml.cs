using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Forms;

namespace WPFFrameworkApp
{
    /// <summary>
    /// FontSettings.xaml etkileşim mantığı
    /// </summary>
    public partial class FontSettings : Window
    {
        public static string configFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), Configs.C_CONFIGS);
        private static string selectedDesktopFont;
        private static string selectedDesktopFontColor;
        private static string selectedDesktopFontWeight;
        private static string selectedDesktopFontStyle;
        private static float selectedDesktopFontSize;

        private static string selectedFolderFont;
        private static string selectedFolderFontColor;
        private static string selectedFolderFontWeight;
        private static string selectedFolderFontStyle;
        private static float selectedFolderFontSize;

        private static string selectedMenuFont;
        private static string selectedMenuFontColor;
        private static string selectedMenuFontWeight;
        private static string selectedMenuFontStyle;
        private static float selectedMenuFontSize;

        public FontSettings()
        {
            InitializeComponent();
            ApplyAllFontSettings();
            SetStyles();
            Show();
        }

        #region MenuStyle functions
        private void SetStyles()
        {
            System.Windows.Controls.MenuItem[] items = { FItem1, FItem2, FItem3 };
            RoutineLogics.SetWindowStyles(fontmenu, items);
        }
        #endregion

        #region Font change functions
        private void ChooseDesktopFont_Click(object sender, RoutedEventArgs e)
        {
            ChangeFontOf(desktopFontPreview, 0);
        }
        private void ChooseFolderFont_Click(object sender, RoutedEventArgs e)
        {
            ChangeFontOf(folderFontPreview, 1);
        }
        private void ChooseMenuFont_Click(object sender, RoutedEventArgs e)
        {
            ChangeFontOf(menuFontPreview, 2);
        }
        private void ChangeFontOf(System.Windows.Controls.TextBox textbox, short which)
        {
            using (FontDialog fontDialog = new FontDialog())
            {
                fontDialog.ShowColor = true; // Renk seçimini de etkinleştirir
                fontDialog.Font = new System.Drawing.Font(textbox.FontFamily.ToString(), (float)textbox.FontSize);

                if (fontDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    textbox.FontFamily = new FontFamily(fontDialog.Font.FontFamily.Name);
                    textbox.FontSize = fontDialog.Font.Size;
                    textbox.FontWeight = fontDialog.Font.Bold ? FontWeights.Bold : FontWeights.Regular;
                    textbox.FontStyle = fontDialog.Font.Italic ? FontStyles.Italic : FontStyles.Normal;
                    textbox.Foreground = new SolidColorBrush(Color.FromArgb(fontDialog.Color.A, fontDialog.Color.R, fontDialog.Color.G, fontDialog.Color.B));
                    textbox.Text = $"{fontDialog.Font.Name}, {fontDialog.Font.Size}pt";

                    switch (which)
                    {
                        case 0: SetDesktopFontSettings(fontDialog); break;
                        case 1: SetFolderFontSettings(fontDialog); break;
                        case 2: SetMenuFontSettings(fontDialog); break;
                    }

                }
            }
        }
        #endregion

        #region Apply settings functions
        private void ApplyAllFontSettings()
        {
            string[] fontsSettings = File.ReadAllLines(Path.Combine(configFolder, Configs.CFONT));
            ApplyCurrentDesktopFontSettings(fontsSettings);
            ApplyCurrentFolderFontSettings(fontsSettings);
            ApplyCurrentMenuFontSettings(fontsSettings);
        }
        private void ApplyCurrentDesktopFontSettings(string[] fontsettings)
        {
            try
            {
                desktopFontPreview.FontFamily = new FontFamily(fontsettings[0]);
                desktopFontPreview.Foreground = RoutineLogics.ConvertHexColor(fontsettings[1]);
                desktopFontPreview.FontWeight = fontsettings[2] == "Bold" ? FontWeights.Bold : FontWeights.Regular;
                desktopFontPreview.FontStyle = fontsettings[3] == "Italic" ? FontStyles.Italic : FontStyles.Normal;
                desktopFontPreview.FontSize = float.Parse(fontsettings[4]);
                desktopFontPreview.Text = $"{fontsettings[0]}, {fontsettings[4]}pt";

                selectedDesktopFont = fontsettings[0];
                selectedDesktopFontColor = fontsettings[1];
                selectedDesktopFontWeight = fontsettings[2];
                selectedDesktopFontStyle = fontsettings[3];
                selectedDesktopFontSize = float.Parse(fontsettings[4]);

                RoutineLogics.desktopFontcolor = fontsettings[1];

            }
            catch (Exception)
            {
                System.Windows.MessageBox.Show("An error occured while configuring desktop font settings.\nDefault font settings will be used.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                SetFontSettingsDefaults();
            }
        }
        private void ApplyCurrentFolderFontSettings(string[] fontsettings)
        {
            try
            {
                folderFontPreview.FontFamily = new FontFamily(fontsettings[5]);
                folderFontPreview.Foreground = RoutineLogics.ConvertHexColor(fontsettings[6]);
                folderFontPreview.FontWeight = fontsettings[7] == "Bold" ? FontWeights.Bold : FontWeights.Regular;
                folderFontPreview.FontStyle = fontsettings[8] == "Italic" ? FontStyles.Italic : FontStyles.Normal;
                folderFontPreview.FontSize = float.Parse(fontsettings[9]);
                folderFontPreview.Text = $"{fontsettings[5]}, {fontsettings[9]}pt";

                selectedFolderFont = fontsettings[5];
                selectedFolderFontColor = fontsettings[6];
                selectedFolderFontWeight = fontsettings[7];
                selectedFolderFontStyle = fontsettings[8];
                selectedFolderFontSize = float.Parse(fontsettings[9]);

                RoutineLogics.folderFontcolor = fontsettings[6];
            }
            catch (Exception)
            {
                System.Windows.MessageBox.Show("An error occured while configuring folder font settings.\nDefault font settings will be used.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                SetFontSettingsDefaults();
            }
        }
        private void ApplyCurrentMenuFontSettings(string[] fontsettings)
        {
            try
            {
                menuFontPreview.FontFamily = new FontFamily(fontsettings[10]);
                menuFontPreview.Foreground = RoutineLogics.ConvertHexColor(fontsettings[11]);
                menuFontPreview.FontWeight = fontsettings[12] == "Bold" ? FontWeights.Bold : FontWeights.Regular;
                menuFontPreview.FontStyle = fontsettings[13] == "Italic" ? FontStyles.Italic : FontStyles.Normal;
                menuFontPreview.FontSize = float.Parse(fontsettings[14]);
                menuFontPreview.Text = $"{fontsettings[10]}, {fontsettings[14]}pt";

                selectedMenuFont = fontsettings[10];
                selectedMenuFontColor = fontsettings[11];
                selectedMenuFontWeight = fontsettings[12];
                selectedMenuFontStyle = fontsettings[13];
                selectedMenuFontSize = float.Parse(fontsettings[14]);

                RoutineLogics.menuFontColor = fontsettings[11];
            }
            catch (Exception)
            {
                System.Windows.MessageBox.Show("An error occured while configuring menu font settings.\nDefault font settings will be used.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                SetFontSettingsDefaults();
            }
        }
        #endregion

        #region SetFontSettings functions
        private void SetDesktopFontSettings(FontDialog fontDialog)
        {
            SetSettings(fontDialog, selectedDesktopFont, selectedDesktopFontColor, selectedDesktopFontWeight, selectedDesktopFontStyle, selectedDesktopFontSize);
            RoutineLogics.desktopFontcolor = selectedDesktopFontColor;
        }
        private void SetFolderFontSettings(FontDialog fontDialog)
        {
            SetSettings(fontDialog, selectedFolderFont, selectedFolderFontColor, selectedFolderFontWeight, selectedFolderFontStyle, selectedFolderFontSize);
            RoutineLogics.folderFontcolor = selectedFolderFontColor;
        }
        private void SetMenuFontSettings(FontDialog fontDialog)
        {
            SetSettings(fontDialog, selectedMenuFont, selectedMenuFontColor, selectedMenuFontWeight, selectedMenuFontStyle, selectedMenuFontSize);
            RoutineLogics.menuFontColor = selectedMenuFontColor;
        }
        private void SetSettings(FontDialog fontDialog, string selectedFont, string selectedFontColor, string selectedFontWeight, string selectedFontStyle, float selectedFontSize)
        {
            selectedFont = fontDialog.Font.Name;
            selectedFontColor = $"#{fontDialog.Color.R:X2}{fontDialog.Color.G:X2}{fontDialog.Color.B:X2}";
            selectedFontWeight = fontDialog.Font.Bold ? "Bold" : "Regular";
            selectedFontStyle = fontDialog.Font.Italic ? "Italic" : "Normal";
            selectedFontSize = (float)fontDialog.Font.Size;
        }
        private void SetFontSettingsDefaults()
        {
            SetDesktopDefault();
            SetFolderDefault();
            SetMenuDefault();
        }
        private void SetDesktopDefault()
        {
            selectedDesktopFont = Defaults.FONT;
            selectedDesktopFontColor = Defaults.FONT_COL;
            selectedDesktopFontWeight = Defaults.FONT_WEIGHT;
            selectedDesktopFontStyle = Defaults.FONT_STYLE;
            selectedDesktopFontSize = int.Parse(Defaults.FONT_SIZE);

            desktopFontPreview.FontFamily = new FontFamily(selectedDesktopFont);
            desktopFontPreview.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(selectedDesktopFontColor));
            desktopFontPreview.FontWeight = selectedDesktopFontWeight == "Bold" ? FontWeights.Bold : FontWeights.Regular;
            desktopFontPreview.FontStyle = selectedDesktopFontStyle == "Italic" ? FontStyles.Italic : FontStyles.Normal;
            desktopFontPreview.FontSize = selectedDesktopFontSize;
            desktopFontPreview.Text = $"{selectedDesktopFont}, {selectedDesktopFontSize}pt";

            RoutineLogics.desktopFontcolor = selectedDesktopFontColor;
        }
        private void SetFolderDefault()
        {
            selectedFolderFont = Defaults.FONT;
            selectedFolderFontColor = Defaults.FONT_COL;
            selectedFolderFontWeight = Defaults.FONT_WEIGHT;
            selectedFolderFontStyle = Defaults.FONT_STYLE;
            selectedFolderFontSize = int.Parse(Defaults.FONT_SIZE);

            folderFontPreview.FontFamily = new FontFamily(selectedFolderFont);
            folderFontPreview.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(selectedFolderFontColor));
            folderFontPreview.FontWeight = selectedFolderFontWeight == "Bold" ? FontWeights.Bold : FontWeights.Regular;
            folderFontPreview.FontStyle = selectedFolderFontStyle == "Italic" ? FontStyles.Italic : FontStyles.Normal;
            folderFontPreview.FontSize = selectedFolderFontSize;
            folderFontPreview.Text = $"{selectedFolderFont}, {selectedFolderFontSize}pt";

            RoutineLogics.folderFontcolor = selectedFolderFontColor;
        }
        private void SetMenuDefault()
        {
            selectedMenuFont = Defaults.FONT;
            selectedMenuFontColor = Defaults.FONT_COL;
            selectedMenuFontWeight = Defaults.FONT_WEIGHT;
            selectedMenuFontStyle = Defaults.FONT_STYLE;
            selectedMenuFontSize = int.Parse(Defaults.FONT_SIZE);

            menuFontPreview.FontFamily = new FontFamily(selectedMenuFont);
            menuFontPreview.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(selectedMenuFontColor));
            menuFontPreview.FontWeight = selectedMenuFontWeight == "Bold" ? FontWeights.Bold : FontWeights.Regular;
            menuFontPreview.FontStyle = selectedMenuFontStyle == "Italic" ? FontStyles.Italic : FontStyles.Normal;
            menuFontPreview.FontSize = selectedMenuFontSize;
            menuFontPreview.Text = $"{selectedMenuFont}, {selectedMenuFontSize}pt";

            RoutineLogics.menuFontColor = selectedMenuFontColor;
        }
        #endregion

        #region GetFontSettings functions
        private string[] GetDeskopFontSettings(string[] allfontsettings)
        {
            selectedDesktopFont = desktopFontPreview.FontFamily.ToString();
            selectedDesktopFontColor = desktopFontPreview.Foreground.ToString();
            selectedDesktopFontWeight = desktopFontPreview.FontWeight.ToString();
            selectedDesktopFontStyle = desktopFontPreview.FontStyle.ToString();
            selectedDesktopFontSize = (float)desktopFontPreview.FontSize;

            allfontsettings[0] = selectedDesktopFont;
            allfontsettings[1] = selectedDesktopFontColor;
            allfontsettings[2] = selectedDesktopFontWeight;
            allfontsettings[3] = selectedDesktopFontStyle;
            allfontsettings[4] = selectedDesktopFontSize.ToString();

            return allfontsettings;
        }
        private string[] GetFolderFontSettings(string[] allfontsettings)
        {
            selectedFolderFont = folderFontPreview.FontFamily.ToString();
            selectedFolderFontColor = folderFontPreview.Foreground.ToString();
            selectedFolderFontWeight = folderFontPreview.FontWeight.ToString();
            selectedFolderFontStyle = folderFontPreview.FontStyle.ToString();
            selectedFolderFontSize = (float)folderFontPreview.FontSize;

            allfontsettings[5] = selectedFolderFont;
            allfontsettings[6] = selectedFolderFontColor;
            allfontsettings[7] = selectedFolderFontWeight;
            allfontsettings[8] = selectedFolderFontStyle;
            allfontsettings[9] = selectedFolderFontSize.ToString();

            return allfontsettings;
        }
        private string[] GetMenuFontSettings(string[] allfontsettings)
        {
            selectedMenuFont = menuFontPreview.FontFamily.ToString();
            selectedMenuFontColor = menuFontPreview.Foreground.ToString();
            selectedMenuFontWeight = menuFontPreview.FontWeight.ToString();
            selectedMenuFontStyle = menuFontPreview.FontStyle.ToString();
            selectedMenuFontSize = (float)menuFontPreview.FontSize;

            allfontsettings[10] = selectedMenuFont;
            allfontsettings[11] = selectedMenuFontColor;
            allfontsettings[12] = selectedMenuFontWeight;
            allfontsettings[13] = selectedMenuFontStyle;
            allfontsettings[14] = selectedMenuFontSize.ToString();

            return allfontsettings;
        }
        #endregion

        #region FontSettings menuitems functions
        private void ApplyChanges_Clicked(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(selectedDesktopFont) || string.IsNullOrEmpty(selectedFolderFont) || string.IsNullOrEmpty(selectedMenuFont))
            {
                System.Windows.MessageBox.Show("You can not apply emtpy font", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string[] fontsSettings = new string[15];

            fontsSettings = GetDeskopFontSettings(fontsSettings);
            fontsSettings = GetFolderFontSettings(fontsSettings);
            fontsSettings = GetMenuFontSettings(fontsSettings);

            File.WriteAllLines(Path.Combine(configFolder, Configs.CFONT), fontsSettings);

            RoutineLogics.ReloadNeededForEveryWindow();
            Close();
        }
        private void CancelChanges_Clicked(object sender, RoutedEventArgs e)
        {
            if (System.Windows.MessageBox.Show("Are you sure to quit without saving?", "Cancel Changes", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Close();
            }
        }
        private void RestoreDefaults_Wanted(object sender, RoutedEventArgs e)
        {
            if (System.Windows.MessageBox.Show("Do you really want to restore default settings?", "Restore Defaults", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                SetFontSettingsDefaults();
            }
        }
        #endregion
    }
}
