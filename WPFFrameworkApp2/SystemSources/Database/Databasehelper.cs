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
                    string sql = "CREATE TABLE IF NOT EXISTS Users (Username TEXT NOT NULL UNIQUE, Password TEXT NOT NULL)";
                    SQLiteCommand command = new SQLiteCommand(sql, connection);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static bool ValidateUser(string username, string password)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM Users WHERE Username=@username AND Password=@password";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@password", HashPassword(password));
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        public static bool RegisterUser(string username, string password)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string checkQuery = "SELECT COUNT(*) FROM Users WHERE Username = @username";
                using (var checkCommand = new SQLiteCommand(checkQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue("@username", username);
                    int userCount = Convert.ToInt32(checkCommand.ExecuteScalar());

                    if (userCount > 0)
                    {
                        return false;
                    }
                }

                string insertQuery = "INSERT INTO Users (Username, Password) VALUES (@username, @password)";
                using (var insertCommand = new SQLiteCommand(insertQuery, connection))
                {
                    insertCommand.Parameters.AddWithValue("@username", username);
                    insertCommand.Parameters.AddWithValue("@password", HashPassword(password));
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


