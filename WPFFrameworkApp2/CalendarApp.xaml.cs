using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
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

            calendar.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(colors[0]));
            rectangle.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(colors[1]));

        }
    }
}
