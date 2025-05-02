using System.Windows;
using System.Windows.Input;

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
