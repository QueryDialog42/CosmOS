using System;
using System.Text;
using System.Windows;
using WPFFrameworkApp;
using System.Data.SQLite;
using System.Windows.Threading;

namespace WPFFrameworkApp2
{
    /// <summary>
    /// LoginWindow.xaml etkileşim mantığı
    /// </summary>
    public partial class LoginWindow : Window
    {
        DispatcherTimer timer;
        private bool continueAllowed = false;
        public static string connectionString = "Data source=systemusers.db;Versions=3;";
        public LoginWindow()
        {
            InitializeComponent();
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
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var connection = new SQLiteConnection(connectionString);

            connection.Open();

            var query = new SQLiteCommand($"SELECT * FROM users WHERE username = '{usernamebox.usertextbox.Text}' AND password = '{RegisterWindow.HashPassword(passwordBox.Password)}';", connection);
            using (var reader = query.ExecuteReader())
            {
                int rowCount = 0;
                while (reader.Read())
                {
                    rowCount++;
                }

                if (rowCount > 0)
                {
                    MainWindow.LoggedIn = true;
                    continueAllowed = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("username or password is wrong");
                }
            }
            connection.Close();
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
