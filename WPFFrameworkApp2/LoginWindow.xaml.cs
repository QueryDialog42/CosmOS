using System.Windows;

namespace WPFFrameworkApp2
{
    /// <summary>
    /// LoginWindow.xaml etkileşim mantığı
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            ShowDialog();
        }

        private void Register_ButtonClicked(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Close();
            new RegisterWindow();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // login codes here
        }
    }
}
