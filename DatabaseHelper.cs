using MySql.Data.MySqlClient;

namespace HR_Applicant_System
{
    public static class DatabaseHelper
    {
        private static readonly string ConnectionString = "Server=localhost;Port=3306;Database=hr_capstone_db;Uid=root;Pwd=;";

        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }
    }
}