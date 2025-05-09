using System;
using System.IO;
using System.Windows;
using WPFFrameworkApp;
using System.Net.Http;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Controls;

namespace WPFFrameworkApp2
{
    /// <summary>
    /// CalendarApp.xaml etkileşim mantığı
    /// </summary>
    public partial class CalendarApp : Window
    {
        private string API_KEY;
        public CalendarApp()
        {
            InitializeComponent();
            GetWeatherApiKey();
            SetStyles();
            Show();
        }

        #region SetStyles functions
        private void SetStyles()
        {
            string[] colors = RoutineLogics.GetColorSettingsFromCcol();
            string[] fonts = RoutineLogics.GetFontSettingsFromCfont();

            background.Background = RoutineLogics.ConvertHexColor(colors[1]);

            TextBlock[] texts = { location, explanation, derece, apiwarning};
            foreach (TextBlock text in texts)
            {
                text.Foreground = RoutineLogics.ConvertHexColor(fonts[6]);
            }

            usertexboxcontrol.usertextblock.FontSize = 15;
            usertexboxcontrol.grid.Background = Brushes.Transparent;
        }
        private void SetWeatherWindow()
        {
            apikeyRequired.Visibility = Visibility.Collapsed;
            weatherwindow.Visibility = Visibility.Visible;
        }
        #endregion

        #region Get WeatherInfos functions
        private void GetWeatherApiKey()
        {
            string apifile = Path.Combine(RoutineLogics.configFolder, Configs.CAPI);
            if (File.Exists(apifile) == false)
            {
                File.Create(apifile);
                return;
            }
            else
            {
                string[] api_key = File.ReadAllLines(apifile);
                if (api_key.Length == 0) return;
                else if (string.IsNullOrEmpty(api_key[0]))
                {
                    return;
                }
                else
                {
                    API_KEY = api_key[0];
                    if (api_key.Length > 1) GetWeatherInfo(api_key[1]);
                    else GetWeatherInfo();
                }
            } 
        }
        private void GetWeatherInfo(string city = "İstanbul")
        {
            using (var httpclient = new HttpClient())
            {
                try
                {
                    string url = $"http://api.openweathermap.org/data/2.5/weather?q={city}&appid={API_KEY}";
                    string json = httpclient.GetStringAsync(url).Result;

                    WeatherInfo.root weatherInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<WeatherInfo.root>(json);

                    derece.Text = (weatherInfo.main.temp - 273.15).ToString("0.00") + " °C";
                    explanation.Text = weatherInfo.weather[0].description;

                    int id = weatherInfo.weather[0].id;

                    if (200 <= id && id <= 232) weathericon.Source = RoutineLogics.setBitmapImage(ImagePaths.STORM_RAINNY);
                    else if (300 <= id && id <= 321) weathericon.Source = RoutineLogics.setBitmapImage(ImagePaths.DRIZZLE_IMG);
                    else if (500 <= id && id <= 531) weathericon.Source = RoutineLogics.setBitmapImage(ImagePaths.RAINY_IMG);
                    else if (600 <= id && id <= 622) weathericon.Source = RoutineLogics.setBitmapImage(ImagePaths.SNOWY_IMG);
                    else if (701 <= id && id < 741) weathericon.Source = RoutineLogics.setBitmapImage(ImagePaths.CLOUD_FOG_IMG);
                    else if (id == 762) weathericon.Source = RoutineLogics.setBitmapImage(ImagePaths.CLOUD_DARK_FOG_IMG);
                    else if (id == 771) weathericon.Source = RoutineLogics.setBitmapImage(ImagePaths.LIGHTNING_IMG);
                    else if (id == 781) weathericon.Source = RoutineLogics.setBitmapImage(ImagePaths.STORM_IMG);
                    else if (id == 800) weathericon.Source = RoutineLogics.setBitmapImage(ImagePaths.SUNNY_IMG);
                    else if (801 <= id && id <= 804) weathericon.Source = RoutineLogics.setBitmapImage(ImagePaths.DARK_CLOUDY_IMG);
                    else weathericon.Source = RoutineLogics.setBitmapImage(ImagePaths.UNKNOWN_IMG);

                    location.Text = $"{weatherInfo.sys.country}, {weatherInfo.name}";

                    string[] lines = {API_KEY, weatherInfo.name };
                    File.WriteAllLines(Path.Combine(RoutineLogics.configFolder, Configs.CAPI), lines);

                    SetWeatherWindow();

                } catch(Exception) 
                {
                    MessageBox.Show("Your API key is invalid or not activated yet.\nIf you sure about your API key is right,\nwait until it is activated.", "API key error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
        #endregion

        #region Button click handler functions
        private void GetWeatherButton_Click(object sender, RoutedEventArgs e)
        {
            API_KEY = Api_Key.Text;
            SetWeatherWindow();
            GetWeatherInfo();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GetWeatherInfo(usertexboxcontrol.usertextbox.Text);
        }
        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }
        #endregion
    }
}
