using System;
using System.Data.SQLite;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Input;
using WPFFrameworkApp;

namespace WPFFrameworkApp2
{
    /// <summary>
    /// RegisterWindow.xaml etkileşim mantığı
    /// </summary>
    public partial class RegisterWindow : Window
    {
        private bool continueAllowed = false;

        public RegisterWindow()
        {
            InitializeComponent();
            ShowDialog();
        }

        private void HaveAnAccount_ButtonClicked(object sender, MouseButtonEventArgs e)
        {
            continueAllowed = true;
            Close();
            new LoginWindow();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var connection = new SQLiteConnection(LoginWindow.connectionString);
                
                connection.Open();
                // var delete = new SQLiteCommand("DELETE FROM users;", connection);
                var insert = new SQLiteCommand($"INSERT INTO users VALUES ('{usernamebox.usertextbox.Text}', '{HashPassword(passwordbox.usertextbox.Text)}');", connection);
                insert.ExecuteNonQuery();

                connection.Close();

                MessageBox.Show("Now try to login with your username and password!", "Registered Succesfully", MessageBoxButton.OK, MessageBoxImage.Information);

                continueAllowed = true;
                Close();
                new LoginWindow();

            } catch (Exception ex)
            {
                RoutineLogics.ErrorMessage("Register Error", "An error occured while registering.\n", ex.Message);
            }
        }

        public static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2")); // Convert to hexadecimal
                }
                return builder.ToString();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (continueAllowed == false) Application.Current.Shutdown();
        }
    }
}
