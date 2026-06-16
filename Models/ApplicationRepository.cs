using HR_Applicant_System;
using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using HR_Applicant_System.ViewModels;

namespace HR_Applicant_System.Models
{
    public class ApplicationRepository
    {
        public List<ApplicantItem> GetAllActiveApplications()
        {
            var list = new List<ApplicantItem>();

            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                // Using JOIN to get the Name from Applicants and Title from JobVacancies
                string query = @"SELECT ap.FullName, j.JobTitle, a.CurrentStatus 
                                FROM applications a
                                INNER JOIN applicants ap ON a.ApplicantID = ap.ApplicantID
                                INNER JOIN jobvacancies j ON a.JobID = j.JobID";

                using (var cmd = new MySqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new ApplicantItem
                        {
                            Name = reader.IsDBNull(reader.GetOrdinal("FullName")) ? string.Empty : reader.GetString("FullName"),
                            Position = reader.IsDBNull(reader.GetOrdinal("JobTitle")) ? string.Empty : reader.GetString("JobTitle"),
                            Status = reader.IsDBNull(reader.GetOrdinal("CurrentStatus")) ? string.Empty : reader.GetString("CurrentStatus")
                        });
                    }
                }
            }
            return list;
        }

        public bool UpdateApplicationStatus(int applicationId, string newStatus)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    // Aligned with the snake_case naming used in GetAllActiveApplications
                    string query = "UPDATE applications SET current_status = @status WHERE application_id = @id";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@status", newStatus);
                        cmd.Parameters.AddWithValue("@id", applicationId);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating application status: {ex.Message}");
                return false;
            }
        }
    }
}