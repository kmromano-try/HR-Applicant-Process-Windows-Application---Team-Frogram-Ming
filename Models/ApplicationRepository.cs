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
                
                // UPDATED: Added a.ApplicationID to pull the unique key for selection tracking
                string query = $@"SELECT a.ApplicationID, ap.FullName, j.JobTitle, a.Status 
                                FROM {DatabaseHelper.ApplicationTable} a
                                INNER JOIN {DatabaseHelper.ApplicantTable} ap ON a.ApplicantID = ap.ApplicantID
                                INNER JOIN {DatabaseHelper.JobTable} j ON a.VacancyID = j.VacancyID"; 

                using (var cmd = new MySqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new ApplicantItem
                        {
                            // UPDATED: Mapping the ApplicationID database column to the item property
                            ApplicationID = reader.IsDBNull(reader.GetOrdinal("ApplicationID")) ? 0 : reader.GetInt32("ApplicationID"),
                            Name = reader.IsDBNull(reader.GetOrdinal("FullName")) ? string.Empty : reader.GetString("FullName"),
                            Position = reader.IsDBNull(reader.GetOrdinal("JobTitle")) ? string.Empty : reader.GetString("JobTitle"),
                            Status = reader.IsDBNull(reader.GetOrdinal("Status")) ? string.Empty : reader.GetString("Status")
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
                    string query = $"UPDATE {DatabaseHelper.ApplicationTable} SET Status = @status WHERE ApplicationID = @id";
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