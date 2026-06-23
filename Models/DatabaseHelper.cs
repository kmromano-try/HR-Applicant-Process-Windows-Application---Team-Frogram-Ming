using MySql.Data.MySqlClient;
using System.Data;

namespace HR_Applicant_System.Models
{
    public static class DatabaseHelper
    {
        private static string ConnectionString = "Server=localhost;Port=3306;Database=hr_applicant_system;Uid=root;Pwd=;";

        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }

        // Method for SELECT queries (returns data)
        public static DataTable ExecuteQuery(string query)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new MySqlCommand(query, conn))
                {
                    using (var adapter = new MySqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        // Method for INSERT, UPDATE, DELETE queries (returns number of rows affected)
        public static int ExecuteNonQuery(string query)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new MySqlCommand(query, conn))
                {
                    return cmd.ExecuteNonQuery();
                }
            }
        }
    }
}