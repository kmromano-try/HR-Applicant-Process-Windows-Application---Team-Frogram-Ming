using MySql.Data.MySqlClient;

namespace HR_Applicant_System
{
    public static class DatabaseHelper
    {
        private static string connectionString = "Server=localhost;Database=hr_capstone_db;Uid=root;Pwd=;";

        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }
    }
}