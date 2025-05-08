using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFFrameworkApp2.UserControls
{
    /// <summary>
    /// TextBoxControls.xaml etkileşim mantığı
    /// </summary>
    public partial class TextBoxControls : UserControl
    {
        public TextBoxControls()
        {
            InitializeComponent();
            DataContext = this;
        }

        private string placeholder;

        public string PlaceHolder
        {
            get { return placeholder; }
            set { placeholder = value; }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(usertextbox.Text)) usertextblock.Visibility = Visibility.Visible;
            else usertextblock.Visibility = Visibility.Hidden;
        }
    }
}
