using AnalyticConfig.Models;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Configuration;

namespace AnalyticConfig.DAL
{
    public class UserDAL
    {
        private static string consString = ConfigurationManager.ConnectionStrings["MySQLConnection"].ConnectionString;
        private MySqlConnection connection = new MySqlConnection(consString);

        public List<User> GetUsers()
        {
            SystemAdmin authUser = AdminDal.GetAuthUser();
            List<User> users = new List<User>();

            using (connection)
            using (var command = connection.CreateCommand())
            {
                connection.Open();

                command.CommandText = "SELECT * FROM user WHERE customer_id = @customer_id";
                command.Parameters.AddWithValue("@customer_id", authUser.CustomerID);

                MySqlDataReader Reader = command.ExecuteReader();

                if (!Reader.HasRows) return null; // Could not find users

                while (Reader.Read())
                {
                    users.Add(new User {
                        Id = Reader.GetInt32("id"),
                        SchoolUnit = Reader.IsDBNull(Reader.GetOrdinal("unit")) ? string.Empty : Reader.GetString("unit"),
                        Name = Reader.IsDBNull(Reader.GetOrdinal("name")) ? string.Empty : Reader.GetString("name"),
                        Email = Reader.IsDBNull(Reader.GetOrdinal("email")) ? string.Empty : Reader.GetString("email"),
                        Password = Reader.IsDBNull(Reader.GetOrdinal("password")) ? string.Empty : Reader.GetString("password"),
                        Role = Reader.IsDBNull(Reader.GetOrdinal("role")) ? string.Empty : Reader.GetString("role"),
                    });
                }
                Reader.Close();
                
            }

            return users;

        }

        public User GetUser(int id)
        {
            User user = new User();

            using (connection)
            using (var command = connection.CreateCommand())
            {
                connection.Open();

                command.CommandText = "SELECT * FROM user WHERE id = @id LIMIT 1";
                command.Parameters.AddWithValue("@id", id);

                MySqlDataReader Reader = command.ExecuteReader();

                if (!Reader.HasRows) return null; // Could not find users

                while (Reader.Read())
                {                 
                    user.Id = Reader.GetInt32("id");
                    user.CustomerID = Reader.GetInt32("customer_id");
                    user.Customer = Reader.IsDBNull(Reader.GetOrdinal("customer")) ? string.Empty : Reader.GetString("customer");
                    user.SchoolUnit = Reader.IsDBNull(Reader.GetOrdinal("unit")) ? string.Empty : Reader.GetString("unit");
                    user.Name = Reader.IsDBNull(Reader.GetOrdinal("name")) ? string.Empty : Reader.GetString("name");
                    user.Email = Reader.IsDBNull(Reader.GetOrdinal("email")) ? string.Empty : Reader.GetString("email");
                    user.Password = Reader.IsDBNull(Reader.GetOrdinal("password")) ? string.Empty : Reader.GetString("password");
                    user.Role = Reader.IsDBNull(Reader.GetOrdinal("role")) ? string.Empty : Reader.GetString("role");             
                }

                Reader.Close();
            }

            return user;

        }

        public User UpdateUser(User user)
        {
            using (connection)
            using (var command = connection.CreateCommand())
            {
                connection.Open();

                command.CommandText = @"UPDATE user
                   SET unit = @unit, name = @name, email = @email, role = @role, password = @password
                   WHERE id = @id LIMIT 1";
                command.Parameters.AddWithValue("@id", user.Id);
                command.Parameters.AddWithValue("@unit", user.SchoolUnit);
                command.Parameters.AddWithValue("@name", user.Name);
                command.Parameters.AddWithValue("@email", user.Email);
                command.Parameters.AddWithValue("@role", user.Role);
                command.Parameters.AddWithValue("@password", user.Password);
                command.ExecuteNonQuery();
            }

            return user;
        }

        public User CreateUser(User user)
        {
            SystemAdmin authUser = AdminDal.GetAuthUser();

            using (connection)
            using (var command = connection.CreateCommand())
            {
                connection.Open();
                command.CommandText =
                @"INSERT INTO user 
                    (customer_id, customer, unit, name, email, role, password) 
                    VALUES 
                    (@customer_id, @customer, @unit, @name, @email, @role, @password)";
                command.Parameters.AddWithValue("@customer_id", authUser.CustomerID);
                command.Parameters.AddWithValue("@customer", authUser.Customer);
                command.Parameters.AddWithValue("@unit", user.SchoolUnit);
                command.Parameters.AddWithValue("@name", user.Name);
                command.Parameters.AddWithValue("@email", user.Email);
                command.Parameters.AddWithValue("@role", user.Role);
                command.Parameters.AddWithValue("@password", user.Password);
                command.ExecuteNonQuery();  
           
            }
            return user;
        }

        public int DeleteUser(int id)
        {
            using (connection)
            using (var command = connection.CreateCommand())
            {
                connection.Open();

                command.CommandText = ("DELETE FROM user WHERE id = @id LIMIT 1");
                command.Parameters.AddWithValue("@id", id);
                command.ExecuteNonQuery();
            }
            return id;
        }
    }
}