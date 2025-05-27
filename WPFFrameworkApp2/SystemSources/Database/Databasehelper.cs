using System;
using System.IO;
using System.Text;
using System.Data.SQLite;
using System.Security.Cryptography;

namespace WPFFrameworkApp2.Database
{
    public class DatabaseHelper
    {
        private static string dbPath = "SystemSources/Database/users.db";
        private static string connectionString = $"Data Source={dbPath};Version=3;";

        public static void InitializeDatabase()
        {
            if (!File.Exists(dbPath))
            {
                SQLiteConnection.CreateFile(dbPath);

                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string sql = "CREATE TABLE IF NOT EXISTS cosmosusers(username TEXT UNIQUE, usermail TEXT UNIQUE, userpass TEXT NOT NULL);";
                    SQLiteCommand command = new SQLiteCommand(sql, connection);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static bool ValidateUser(string usermail, string userpass)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM cosmosusers WHERE usermail=@usermail AND userpass=@userpass";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@usermail", usermail);
                    command.Parameters.AddWithValue("@userpass", HashPassword(userpass));
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        public static bool RegisterUser(string usermail, string userpass)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string checkQuery = "SELECT COUNT(*) FROM cosmosusers WHERE usermail = @usermail";
                using (var checkCommand = new SQLiteCommand(checkQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue("@usermail", usermail);
                    int userCount = Convert.ToInt32(checkCommand.ExecuteScalar());

                    if (userCount > 0)
                    {
                        return false;
                    }
                }

                string insertQuery = "INSERT INTO cosmosusers (usermail, userpass) VALUES (@usermail, @userpass)";
                using (var insertCommand = new SQLiteCommand(insertQuery, connection))
                {
                    insertCommand.Parameters.AddWithValue("@usermail", usermail);
                    insertCommand.Parameters.AddWithValue("@userpass", HashPassword(userpass));
                    insertCommand.ExecuteNonQuery();
                    return true;
                }
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
    }
}


