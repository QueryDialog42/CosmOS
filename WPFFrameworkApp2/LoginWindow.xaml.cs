using System;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using WPFFrameworkApp;

namespace WPFFrameworkApp2
{
    /// <summary>
    /// LoginWindow.xaml etkileşim mantığı
    /// </summary>
    public partial class LoginWindow : Window
    {
        DispatcherTimer timer;
        private bool continueAllowed = false;
        public LoginWindow()
        {
            InitializeComponent();
            Database.DatabaseHelper.InitializeDatabase();
            InitTime();
            ShowDialog();
        }

        #region Closing functions
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (continueAllowed == false) Application.Current.Shutdown();

            timer.Stop();
            timer = null;
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
            continueAllowed = true;
            Close();
            new RegisterWindow();
        }
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = usernameBox.usertextbox.Text.Trim();
            string password = passwordBox.Password.Trim();

            txtLoginErrorMessage.Visibility = Visibility.Collapsed;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                txtLoginErrorMessage.Text = "Please fill all blanks";
                txtLoginErrorMessage.Visibility = Visibility.Visible;
                return;
            }

            bool isValid = Database.DatabaseHelper.ValidateUser(username, password);

            if (isValid)
            {
                MainWindow.LoggedIn = true;
                continueAllowed = true;
                Close();
            }
            else
            {
                txtLoginErrorMessage.Text = "Invalid username or password";
                txtLoginErrorMessage.Visibility = Visibility.Visible;
            }
        }
        #endregion

        #region PasswordBox placeholder functions
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
