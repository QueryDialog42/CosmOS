using System;
using System.IO;
using System.Windows;
using WPFFrameworkApp;
using System.Net.Http;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Controls;
using System.Threading.Tasks;

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

            TextBlock[] texts = { location, explanation, degree, apiwarning};
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
                string url = $"http://api.openweathermap.org/data/2.5/weather?q={city}&appid={API_KEY}";
                try
                {
                    if (CheckInternetConnection() == false) return;

                    string json = httpclient.GetStringAsync(url).Result;
                    WeatherInfo.root weatherInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<WeatherInfo.root>(json);

                    degree.Text = (weatherInfo.main.temp - 273.15).ToString("0.00") + " °C";
                    explanation.Text = "  " + weatherInfo.weather[0].description;

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
                    errorText.Text = string.Empty;

                    SetWeatherWindow();
                }
                catch(AggregateException)
                {
                    HttpResponseMessage response = httpclient.GetAsync(url).Result;
                    switch ((int)response.StatusCode)
                    {
                        case 400: errorText.Text = "Invalid city"; break; // Bad Request
                        case 401: MessageBox.Show("Your API key is invalid or not activated yet.\nIf you are sure about your API key, wait until it is activated.", "API key error", MessageBoxButton.OK, MessageBoxImage.Warning); break;
                        case 403: RoutineLogics.ErrorMessage("API Key blocked", "This API key is forbidden to use"); break; // Forbidden
                        case 404: errorText.Text = "City not found"; break; // Not Found
                        case 500: RoutineLogics.ErrorMessage("Server Error", "Server is down. Please try again later."); break; // Internal Server Error
                        case 502: RoutineLogics.ErrorMessage("Bad Gateway", "Server is down. Please try again later."); break; // Bad Gateway
                        case 503: RoutineLogics.ErrorMessage("Service Unavailable", "Server is down. Please try again later."); break; // Service Unavailable
                        case 504: RoutineLogics.ErrorMessage("Gateway Timeout", "Timeout. Please try again later."); break; // Gateway Timeout
                        default: RoutineLogics.ErrorMessage("Error", "Unknown HTTP error.\nWe recommend that you check your internet connection."); break;
                    }    
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
            string city = usertexboxcontrol.usertextbox.Text;
            if (string.IsNullOrEmpty(city) == false) GetWeatherInfo(city);
        }
        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }
        #endregion

        #region Unclassified private functions
        private bool CheckInternetConnection()
        {
            // Check if the network is available before making the request
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                RoutineLogics.ErrorMessage("No Internet", "Internet connection is not available. Please check your connection.");
                apiwarning.Text = "No internet connection";
                hyperlink.Visibility = Visibility.Collapsed;
                Api_Key.Visibility = Visibility.Collapsed;
                button.Visibility = Visibility.Collapsed;
                return false;
            }
            return true;
        }
        #endregion
    }
}
