using MySql.Data.MySqlClient;

namespace HR_Applicant_System.Models
{
    public static class DatabaseHelper
    {
        private static string connectionString =
            "server=localhost;user=root;password=;database=hr_applicant_system;";

        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }
    }
}