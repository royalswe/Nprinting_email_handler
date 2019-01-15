using System.Linq;
using System.IO;
using System.Configuration;
using MySql.Data.MySqlClient;
using System;

/// <summary>
/// Models that handle all fetch from file to database
/// </summary>

namespace AnalyticConfig.Models
{
    public class FetchFromFile
    {
        public FetchFromFile()
        {
            /// <summary>
            /// loop all .csv files in specified directory path and fetch the information from the files and store it in db.
            /// </summary>
            string dirPath = @"C:\customersCSV";
            string[] files = Directory.GetFiles(dirPath, "*.csv");
            
            foreach (string file in files)
            {
                string[] lines = File.ReadAllLines(file);
                foreach (var line in lines.Skip(1))
                {
                    var data = line.Split(new[] { ';' }, 5);
                    string customerID = data[0].Trim();
                    string customer = data[1].Trim();
                    string unit = data[2].Trim();
                    string unitRole = data[3].Trim();
                    StoreRecordInDb(customerID, customer, unit, unitRole);
                }
            }

        }
  
        private void StoreRecordInDb(string customerID, string customer, string unit, string unitRole)
        {
            string consString = ConfigurationManager.ConnectionStrings["MySQLConnection"].ConnectionString;

            using (var connection = new MySqlConnection(consString))
            using (var command = connection.CreateCommand())
            {
                connection.Open();
                // Check if row already exist
                command.CommandText = string.Format("SELECT COUNT(*) FROM user WHERE unit = '{0}' AND customer_id = {1}", unit, customerID);
                int UserExist = Convert.ToInt32(command.ExecuteScalar());

                if (UserExist < 1)
                {
                    command.CommandText =
                    @"INSERT INTO user 
                        (customer_id, customer, unit, unit_role) 
                        VALUES 
                        (@customer_id, @customer, @unit, @unit_role)";
                    command.Parameters.AddWithValue("@customer_id", customerID);
                    command.Parameters.AddWithValue("@customer", customer);
                    command.Parameters.AddWithValue("@unit", unit);
                    command.Parameters.AddWithValue("@unit_role", unitRole);
                    command.ExecuteNonQuery();
                }
            }
        }

    }
}