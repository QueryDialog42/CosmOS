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

namespace WPFFrameworkApp2
{
    /// <summary>
    /// RegisterWindow.xaml etkileşim mantığı
    /// </summary>
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
            ShowDialog();
        }

        private void HaveAnAccount_ButtonClicked(object sender, MouseButtonEventArgs e)
        {
            Close();
            new LoginWindow();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // registering codes here
        }
    }
}
