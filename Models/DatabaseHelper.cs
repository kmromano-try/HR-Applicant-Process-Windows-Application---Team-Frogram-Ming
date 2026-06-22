using MySql.Data.MySqlClient;
using System;

namespace HR_Applicant_System.Models
{
    public static class DatabaseHelper
    {
        // Strictly configured for your local setup
        private static string ConnectionString =
"Server=localhost;Port=3306;Database=hr_applicant_system;Uid=root;Pwd=;";

        public static string StaffTable => "Staff_Accounts";
        public static string JobTable => "Job_Listings";
        public static string ApplicationTable => "Applications";
        public static string ApplicantTable => "Applicants";
        public static string HiringDecisionTable => "hiringdecisions";

        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }
    }
}