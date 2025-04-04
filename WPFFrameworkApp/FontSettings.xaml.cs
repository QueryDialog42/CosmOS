using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;

namespace WPFFrameworkApp
{
    /// <summary>
    /// FontSettings.xaml etkileşim mantığı
    /// </summary>
    public partial class FontSettings : Window
    {
        public static string configFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), Configs.C_CONFIGS);
        private static string selectedFont;
        private static string selectedFontColor;
        private static string selectedFontWeight;
        private static string selectedFontStyle;
        private static int selectedFontSize;
        public FontSettings()
        {
            InitializeComponent();
            Show();
        }

        private void ChooseFont_Click(object sender, RoutedEventArgs e)
        {
            using (FontDialog fontDialog = new FontDialog())
            {
                fontDialog.ShowColor = true; // Renk seçimini de etkinleştirir
                fontDialog.Font = new System.Drawing.Font(SelectedFont.FontFamily.ToString(), (float)SelectedFont.FontSize);

                if (fontDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    SelectedFont.FontFamily = new FontFamily(fontDialog.Font.FontFamily.Name);
                    SelectedFont.FontSize = fontDialog.Font.Size;
                    SelectedFont.FontWeight = fontDialog.Font.Bold ? FontWeights.Bold : FontWeights.Regular;
                    SelectedFont.FontStyle = fontDialog.Font.Italic ? FontStyles.Italic : FontStyles.Normal;
                    SelectedFont.Foreground = new SolidColorBrush(Color.FromArgb(fontDialog.Color.A, fontDialog.Color.R, fontDialog.Color.G, fontDialog.Color.B));
                    SelectedFont.Text = $"{fontDialog.Font.Name}, {fontDialog.Font.Size}pt";

                    selectedFont = fontDialog.Font.Name;
                    selectedFontColor = $"#{fontDialog.Color.R:X2}{fontDialog.Color.G:X2}{fontDialog.Color.B:X2}";
                    selectedFontWeight = fontDialog.Font.Bold ? "Bold" : "Regular";
                    selectedFontStyle = fontDialog.Font.Bold ? "Italic" : "Normal";
                    selectedFontSize = (int)fontDialog.Font.Size;

                    RoutineLogics.fontcolor = selectedFontColor;
                }
            }
        }

        private void ApplyChanges_Clicked(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(selectedFont))
            {
                System.Windows.MessageBox.Show("You can not apply emtpy font", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            string[] fontsSettings = new string[5];
            fontsSettings[0] = selectedFont;
            fontsSettings[1] = selectedFontColor;
            fontsSettings[2] = selectedFontWeight;
            fontsSettings[3] = selectedFontStyle;
            fontsSettings[4] = selectedFontSize.ToString();
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
                SelectedFont.FontFamily = new FontFamily(Defaults.FONT);
                SelectedFont.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Defaults.FONT_COL));
                SelectedFont.FontWeight = FontWeights.Regular;
                SelectedFont.FontStyle = FontStyles.Normal;
                SelectedFont.FontSize = float.Parse(Defaults.FONT_SIZE);
                SelectedFont.Text = $"{Defaults.FONT}, {Defaults.FONT_SIZE}pt";

                selectedFont = Defaults.FONT;
                selectedFontColor = Defaults.FONT_COL;
                selectedFontWeight = Defaults.FONT_WEIGHT;
                selectedFontStyle = Defaults.FONT_STYLE;
                selectedFontSize = int.Parse(Defaults.FONT_SIZE);

                RoutineLogics.fontcolor = selectedFontColor;
            }
        }
    }
}
