using System;
using System.Windows;
using System.Net.Mail;
using MessageBox = System.Windows.MessageBox;
using System.Diagnostics;
using System.Windows.Navigation;
using System.Windows.Media;
using System.Windows.Controls;


namespace WPFFrameworkApp
{
    public partial class GenMailApp : Window
    {
        public GenMailApp()
        {
            InitializeComponent();
            setStyles();
            Show();
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string fromEmail = FromEmailTextBox.Text; 
            string toEmail = ToEmailTextBox.Text;       
            string topic = TopicTextBox.Text;        
            string explanation = ExplanationTextBox.Text;
            string appPassword = AppPasswordTextBox.Text; // Uygulama şifresi

            if (string.IsNullOrWhiteSpace(fromEmail) || string.IsNullOrWhiteSpace(toEmail) || string.IsNullOrWhiteSpace(topic) || string.IsNullOrWhiteSpace(explanation))
            {
                MessageBox.Show("Please fill in all fields.", "WARNING!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!IsValidEmail(toEmail))
            {
                MessageBox.Show("Please enter a valid sender email address.", "WARNING!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!IsValidEmail(fromEmail))
            {
                MessageBox.Show("Please enter a valid recipient email address.", "WARNING!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                // SMTP Ayarları
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new System.Net.NetworkCredential(fromEmail, appPassword.Trim()),
                    EnableSsl = true
                };

                // Mail Ayarları
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail), 
                    Subject = topic,                   
                    Body = explanation,                
                    IsBodyHtml = false // HTML formatında değil
                };
                mailMessage.To.Add(toEmail); 

                smtpClient.Send(mailMessage); 
                MessageBox.Show("Mail has been sent successfully!", "SUCCEED!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred in sending an e-mail: {ex.Message}", "WARNING!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to clean up the fields?", "APPROVED!", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                ToEmailTextBox.Clear();
                FromEmailTextBox.Clear();
                TopicTextBox.Clear();
                ExplanationTextBox.Clear();
                AppPasswordTextBox.Clear();
            }
            else
            {
                MessageBox.Show("The cleaning process has been canceled.", "INFORMATION", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void Hyperlink_Navigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }

        private void setStyles()
        {
            string[] colorsettings = RoutineLogics.GetColorSettingsFromCcol();
            string[] fontsettings = RoutineLogics.GetFontSettingsFromCfont();

            var desktopColor = RoutineLogics.ConvertHexColor(colorsettings[0]);
            var desktopFontColor = RoutineLogics.ConvertHexColor(fontsettings[1]);
            Background = desktopColor;

            TextBlock[] textblocks = { GItem1, GItem2, GItem3, GItem4, GItem5, GItem6};
            TextBox[] textboxes = {ToEmailTextBox, FromEmailTextBox, TopicTextBox, ExplanationTextBox, AppPasswordTextBox };
            foreach (TextBlock textblock in textblocks) textblock.Foreground = desktopFontColor;
            foreach(TextBox textbox in textboxes)
            {
                textbox.Background = Brushes.Transparent;
                textbox.Foreground = desktopFontColor;
            }

            Title = AppTitles.APP_MAIL;
            GItem1.Text = AppTitles.APP_MAIL;
        }
    }
}