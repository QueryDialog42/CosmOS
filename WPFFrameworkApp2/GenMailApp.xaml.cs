using System;
using System.Windows;
using System.Net.Mail;
using MessageBox = System.Windows.MessageBox; // Use the WPF MessageBox


namespace WPFFrameworkApp
{
    public partial class GenMailApp : Window
    {
        public GenMailApp()
        {
            InitializeComponent();
            Show();
        }

        // SendButton_Click: Gönder Butonu İşlemleri
        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            // TextBox'lardaki bilgileri alıyoruz
            string fromEmail = FromEmailTextBox.Text; // Gönderen e-posta adresi
            string email = EmailTextBox.Text;        // Alıcı e-posta adresi
            string topic = TopicTextBox.Text;        // Konu
            string explanation = ExplanationTextBox.Text; // Açıklama

            // Boş alanları kontrol et
            if (string.IsNullOrWhiteSpace(fromEmail) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(topic) || string.IsNullOrWhiteSpace(explanation))
            {
                MessageBox.Show("Lütfen tüm alanları doldurun.", "UYARI!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Gönderen ve Alıcı e-posta adreslerinin geçerlilik kontrolü
            if (!IsValidEmail(fromEmail))
            {
                MessageBox.Show("Geçerli bir gönderen e-posta adresi giriniz.", "UYARI!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!IsValidEmail(email))
            {
                MessageBox.Show("Geçerli bir alıcı e-posta adresi giriniz.", "UYARI!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                // SMTP Ayarları
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new System.Net.NetworkCredential(fromEmail, ""), // Gönderen e-posta şifresi (Güvenlik için app.config kullanılabilir)
                    EnableSsl = true
                };

                // Mail Ayarları
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail), // Gönderen adres
                    Subject = topic,                   // Konu
                    Body = explanation,                // Açıklama
                    IsBodyHtml = false                 // HTML formatında değil
                };
                mailMessage.To.Add(email); // Alıcı adresi

                smtpClient.Send(mailMessage); // Mail gönder
                MessageBox.Show("Mail başarıyla gönderildi!", "Başarılı!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Mail gönderiminde bir hata oluştu: {ex.Message}", "UYARI!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // CancelButton_Click: İptal Butonu İşlemleri
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Alanları temizlemek istediğinize emin misiniz?", "Onay", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                // Tüm TextBox'ları temizle
                EmailTextBox.Clear();
                FromEmailTextBox.Clear();
                TopicTextBox.Clear();
                ExplanationTextBox.Clear();

                MessageBox.Show("Alanlar temizlendi.", "Bilgilendirme", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Temizlik işlemi iptal edildi.", "Bilgilendirme", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // E-posta Geçerlilik Kontrol Fonksiyonu
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
    }
}

