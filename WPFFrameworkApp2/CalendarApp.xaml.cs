using System.Windows;
using WPFFrameworkApp;

namespace WPFFrameworkApp2
{
    /// <summary>
    /// CalendarApp.xaml etkileşim mantığı
    /// </summary>
    public partial class CalendarApp : Window
    {
        public CalendarApp()
        {
            InitializeComponent();
            SetStyles();
            Show();
        }

        private void SetStyles()
        {
            string[] colors = RoutineLogics.GetColorSettingsFromCcol();
            string font = RoutineLogics.GetFontSettingsFromCfont()[1];

            calendar.Background = RoutineLogics.ConvertHexColor(colors[0]);
            rectangle.Fill = RoutineLogics.ConvertHexColor(colors[1]);

        }
    }
}
