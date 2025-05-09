using System;
using System.Collections.Generic;
using System.Data.SQLite;
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
using System.Windows.Shapes;

namespace WPFFrameworkApp2.Database
{
    /// <summary>
    /// Window1.xaml etkileşim mantığı
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }
    }
    public partial class UserListWindow : Window
    {
        private static string dbPath = "Database/users.db";
        private static string connectionString = $"Data Source={dbPath};Version=3;";

        public UserListWindow(object userListBox)
        {
            InitializeComponent();
            LoadUsers();
        }

        private void InitializeComponent()
        {
            throw new NotImplementedException();
        }

        public object UserListBox { get; private set; }

        private void LoadUsers()
        {
            List<string> users = new List<string>();

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT Username FROM Users";
                using (var command = new SQLiteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(reader.GetString(0));
                    }
                }
            }

        }
    }
}
