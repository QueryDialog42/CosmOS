using System.Windows.Input;
using System.Windows.Controls;

namespace WPFFrameworkApp2.UserControls
{
    /// <summary>
    /// HoverableButtonControls.xaml etkileşim mantığı
    /// </summary>
    public partial class HoverableButtonControls : UserControl
    {
        public HoverableButtonControls()
        {
            DataContext = this;
            InitializeComponent();
        }

        private string hoveredtext;

        public string HoveredText
        {
            get { return hoveredtext; }
            set 
            { 
                hoveredtext = value;
            }
        }

        private string unhoveredtext;

        public string UnHoveredText
        {
            get { return unhoveredtext; }
            set 
            {
                unhoveredtext = value;
                textblock.Text = unhoveredtext;
            }
        }


        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            textblock.Text = hoveredtext;
        }

        private void button_MouseLeave(object sender, MouseEventArgs e)
        {
            textblock.Text = unhoveredtext;
        }
    }
}
