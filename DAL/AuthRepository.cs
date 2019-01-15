using AnalyticConfig.Models;
using MySql.Data.MySqlClient;
using System.Configuration;

/// <summary>
/// Model that handle authentications
/// </summary>

namespace AnalyticConfig.DAL
{
    public static class AuthRepository
    {
        /// <summary>
        /// Fetch the user credentials for authentication
        /// </summary>
        public static SystemAdmin Authenticate(string username, string password)
        {
            SystemAdmin user = new SystemAdmin();

            string consString = ConfigurationManager.ConnectionStrings["MySQLConnection"].ConnectionString;
            using (var connection = new MySqlConnection(consString))
            using (var command = connection.CreateCommand())
            {
                connection.Open();

                command.CommandText = "SELECT * FROM sys_admin WHERE username = @username LIMIT 1";
                command.Parameters.AddWithValue("@username", username);

                MySqlDataReader Reader = command.ExecuteReader();
                
                if (!Reader.HasRows) return null; // Could not find user
                
                while (Reader.Read())
                {
                    if (!ValidatePassword(password, Reader.GetString("password"))) return null; // Validate hash
                    user.Username = Reader.GetString("username");
                    user.Password = Reader.GetString("password");
                    user.Role = Reader.IsDBNull(Reader.GetOrdinal("role")) ? string.Empty : Reader.GetString("role");
                }
                Reader.Close();
                return user;
            }
        }

        public static string HashPassword(string password)
        {
            string salt = BCrypt.Net.BCrypt.GenerateSalt(12);
            return BCrypt.Net.BCrypt.HashPassword(password, salt);
        }

        public static bool ValidatePassword(string password, string correctHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, correctHash);
        }

    }
}