using AnalyticConfig.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;

/// <summary>
/// CRUD model to handle admins
/// </summary>

namespace AnalyticConfig.DAL
{
    public static class AdminDal
    {
        private static string consString = ConfigurationManager.ConnectionStrings["MySQLConnection"].ConnectionString;

        public static SystemAdmin GetAuthUser()
        {
            SystemAdmin user = new SystemAdmin();

            string username;
            HttpCookie cookie = HttpContext.Current.Request.Cookies["adminCoockie"];
            if (cookie != null)
            {
                username = cookie.Values["username"];
            }
            else
            {
                username = HttpContext.Current.User.Identity.Name;
            }

            using (MySqlConnection connection = new MySqlConnection(consString))
            using (var command = connection.CreateCommand())
            {
                connection.Open();

                command.CommandText = "SELECT * FROM sys_admin WHERE username = @username LIMIT 1";
                command.Parameters.AddWithValue("@username", username);

                MySqlDataReader Reader = command.ExecuteReader();

                if (!Reader.HasRows) return null; // Could not find user

                while (Reader.Read()) // Fetch the information from the user
                {
                    user.Id = Reader.GetInt32("id");
                    user.CustomerID = Reader.GetInt32("customer_id");
                    user.Username = Reader.GetString("username");
                    user.Email = Reader.GetString("email");
                    user.Customer = Reader.GetString("customer");
                    user.Role = Reader.IsDBNull(Reader.GetOrdinal("role")) ? string.Empty : Reader.GetString("role");
                }
                Reader.Close();
                return user;
            }
        }

        public static List<SystemAdmin> GetAdminUsers()
        {
            List<SystemAdmin> users = new List<SystemAdmin>();

            using (MySqlConnection connection = new MySqlConnection(consString))
            using (var command = connection.CreateCommand())
            {
                connection.Open();

                command.CommandText = "SELECT * FROM sys_admin";

                MySqlDataReader Reader = command.ExecuteReader();

                if (!Reader.HasRows) return null; // Could not find users

                while (Reader.Read()) // Fetch data from the admin users
                {
                    users.Add(new SystemAdmin
                    {
                        Id = Reader.GetInt32("id"),
                        CustomerID = Reader["customer_id"] == DBNull.Value ? default(int) : (int)Reader["customer_id"],
                        Customer = Reader.IsDBNull(Reader.GetOrdinal("customer")) ? string.Empty : Reader.GetString("customer"),
                        Username = Reader.IsDBNull(Reader.GetOrdinal("username")) ? string.Empty : Reader.GetString("username"),
                        Email = Reader.IsDBNull(Reader.GetOrdinal("email")) ? string.Empty : Reader.GetString("email"),
                        Role = Reader.IsDBNull(Reader.GetOrdinal("role")) ? string.Empty : Reader.GetString("role"),
                    });
                }
                Reader.Close();

            }

            return users;

        }

        public static SystemAdmin CreateAdmin(SystemAdmin user)
        {
            using (MySqlConnection connection = new MySqlConnection(consString))
            using (var command = connection.CreateCommand())
            {
                connection.Open();
                string hashedPassword = AuthRepository.HashPassword(user.Password);
                command.CommandText =
                @"INSERT INTO sys_admin 
                        (customer_id, customer, username, email, password, role) 
                        VALUES 
                        (@customer_id, @customer, @username, @email, @password, @role)";
                command.Parameters.AddWithValue("@customer_id", user.CustomerID);
                command.Parameters.AddWithValue("@customer", user.Customer);
                command.Parameters.AddWithValue("@username", user.Username);
                command.Parameters.AddWithValue("@email", user.Email);
                command.Parameters.AddWithValue("@password", hashedPassword);
                command.Parameters.AddWithValue("@role", user.Role);
                command.ExecuteNonQuery();
            }
            return user;
        }

        public static SystemAdmin UpdateAdmin(int id, SystemAdmin user)
        {
            using (MySqlConnection connection = new MySqlConnection(consString))
            using (var command = connection.CreateCommand())
            {
                connection.Open();

                // If requested password change on user editing
                if (user.Password != null)
                {
                    string hashedPassword = AuthRepository.HashPassword(user.Password);
                    command.CommandText = @"UPDATE sys_admin
                       SET customer_id = @customer_id, customer = @customer, username = @username, email = @email, password = @password, role = @role
                       WHERE id = @id LIMIT 1";
                    command.Parameters.AddWithValue("@password", hashedPassword);
                }
                else
                {
                    command.CommandText = @"UPDATE sys_admin
                       SET customer_id = @customer_id, customer = @customer, username = @username, email = @email, role = @role
                       WHERE id = @id LIMIT 1";
                }

                command.Parameters.AddWithValue("@customer_id", user.CustomerID);
                command.Parameters.AddWithValue("@customer", user.Customer);
                command.Parameters.AddWithValue("@username", user.Username);
                command.Parameters.AddWithValue("@email", user.Email);
                command.Parameters.AddWithValue("@role", user.Role);
                command.Parameters.AddWithValue("@id", id);
                command.ExecuteNonQuery();
            }

            return user;
        }

        public static int DeleteAdmin(int id)
        {
            using (MySqlConnection connection = new MySqlConnection(consString))
            using (var command = connection.CreateCommand())
            {
                connection.Open();

                command.CommandText = ("DELETE FROM sys_admin WHERE id = @id LIMIT 1");
                command.Parameters.AddWithValue("@id", id);
                command.ExecuteNonQuery();
            }
            return id;
        }
    }
}