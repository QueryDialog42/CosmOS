using System.Windows;
using System.Windows.Input;
using WPFFrameworkApp2.Database;

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

        #region Closing functions
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (continueAllowed == false) Application.Current.Shutdown();
        }
        #endregion

        #region Button event functions
        private void HaveAnAccount_ButtonClicked(object sender, MouseButtonEventArgs e)
        {
            continueAllowed = true;
            Close();
            new LoginWindow();
        }
        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.usertextbox.Text;
            string usermail = txtUsermail.usertextbox.Text;
            string password = txtPassword.usertextbox.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                txtErrorMessage.Text = "Please fill all blanks";
                txtErrorMessage.Visibility = Visibility.Visible;
                return;
            }


            txtErrorMessage.Visibility = Visibility.Collapsed;

            bool success = DatabaseHelper.RegisterUser(username, usermail, password);
            if (success)
            {
                MessageBox.Show("Now try to login with your username and password", "Registered Succesfully", MessageBoxButton.OK, MessageBoxImage.Information);
                continueAllowed = true;
                Close();
                new LoginWindow();
            }
            else
            {
                txtErrorMessage.Text = "Username is already taken or an unkown error occured";
                txtErrorMessage.Visibility = Visibility.Visible;
            }
        }
        #endregion
    }
}
