using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace HR_Applicant_System.Models;

public static class DatabaseHelper
{
    public static string ApplicantTable = "Applications";
    public static string ApplicationTable = "Applications";
    public static string JobTable = "Jobs";
    public static string StaffTable = "HRStaff";

    private static string ConnectionString = "Server=localhost;Port=3306;Database=hr_applicant_system;Uid=root;Pwd=;";

    public static MySqlConnection GetConnection() 
    {
        return new MySqlConnection(ConnectionString);
    }

    public static bool AuthenticateStaff(string username, string password)
    {
        string query = "SELECT COUNT(*) FROM HRStaff WHERE Username = @u AND Password = @p";
        using (var conn = GetConnection())
        {
            conn.Open();
            using (var cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@u", username);
                cmd.Parameters.AddWithValue("@p", password);
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
        }
    }

    // Fixed nullable warnings by adding '?'
    public static int ExecuteNonQuery(string query, MySqlParameter[]? parameters = null)
    {
        using (var conn = GetConnection())
        {
            conn.Open();
            using (var cmd = new MySqlCommand(query, conn))
            {
                if (parameters != null) cmd.Parameters.AddRange(parameters);
                return cmd.ExecuteNonQuery();
            }
        }
    }

    // Fixed nullable warnings by adding '?'
    public static DataTable ExecuteQuery(string query, MySqlParameter[]? parameters = null)
    {
        using (var conn = GetConnection())
        {
            conn.Open();
            using (var cmd = new MySqlCommand(query, conn))
            {
                if (parameters != null) cmd.Parameters.AddRange(parameters);
                using (var adapter = new MySqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }
    }
}