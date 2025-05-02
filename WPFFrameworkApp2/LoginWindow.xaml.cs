using System;
using System.Text;
using System.Windows;
using WPFFrameworkApp;
using System.Windows.Threading;

namespace WPFFrameworkApp2
{
    /// <summary>
    /// LoginWindow.xaml etkileşim mantığı
    /// </summary>
    public partial class LoginWindow : Window
    {
        DispatcherTimer timer;
        public LoginWindow()
        {
            InitializeComponent();
            InitTime();
            ShowDialog();
        }

        #region Closing functions
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            timer.Stop();
            timer = null;
            MainWindow.LoggedIn = true; // temporary, remove after adding database
        }
        #endregion

        #region Datetime functions
        private void InitTime()
        {
            StringBuilder stringbuilder = new StringBuilder();
            date.Text = stringbuilder.Append(DateTime.Now.DayOfWeek).Append(",   ").Append(DateTime.Now.ToString("MMMM")).Append(" ").Append(DateTime.Now.Day).ToString();

            time.Text = DateTime.Now.ToString("hh:mm");
            timer = new DispatcherTimer();
            timer.Tick += UpdateTime;
            timer.Interval = TimeSpan.FromSeconds(10);
        }
        private void UpdateTime(object sender, EventArgs e)
        {
            time.Text = DateTime.Now.ToString("hh:mm");
        }
        #endregion

        #region Button event functions
        private void Register_ButtonClicked(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Close();
            new RegisterWindow();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // login codes here
        }
        #endregion

        #region PasswordBox Placeholder functions
        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            passwordPlaceholder.Text = string.Empty;
        }
        private void PasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(passwordBox.Password)) passwordPlaceholder.Text = "Password here";
        }
        #endregion
    }
}
