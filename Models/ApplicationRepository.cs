using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace HR_Applicant_System.Models
{
    public class ApplicationRepository
    {
        public List<Application> GetAllActiveApplications()
        {
            var applications = new List<Application>();

            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                
                // Real SQL Query mapped precisely to your MySQL Workbench schema columns
                string query = $@"
                    SELECT 
                        a.ApplicationID, 
                        ap.FullName, 
                        j.JobTitle, 
                        a.Status, 
                        a.StaffFeedback
                    FROM {DatabaseHelper.ApplicationTable} a
                    INNER JOIN {DatabaseHelper.ApplicantTable} ap ON a.ApplicantID = ap.ApplicantID
                    INNER JOIN {DatabaseHelper.JobTable} j ON a.VacancyID = j.VacancyID";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            applications.Add(new Application
                            {
                                Id = reader.GetInt32("ApplicationID"),
                                Name = reader.IsDBNull(reader.GetOrdinal("FullName")) ? "Unknown Candidate" : reader.GetString("FullName"),
                                Position = reader.IsDBNull(reader.GetOrdinal("JobTitle")) ? "Unassigned Position" : reader.GetString("JobTitle"),
                                Status = reader.IsDBNull(reader.GetOrdinal("Status")) ? "Pending" : reader.GetString("Status"),
                                HRRemarks = reader.IsDBNull(reader.GetOrdinal("StaffFeedback")) ? string.Empty : reader.GetString("StaffFeedback")
                            });
                        }
                    }
                }
            }

            return applications;
        }

        public void UpdateApplicationStatus(Application application)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                
                // Realigned update schema matching your database keys
                var query = $"UPDATE {DatabaseHelper.ApplicationTable} SET Status = @status, StaffFeedback = @hrRemarks WHERE ApplicationID = @id";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@status", application.Status);
                    cmd.Parameters.AddWithValue("@hrRemarks", application.HRRemarks);
                    cmd.Parameters.AddWithValue("@id", application.Id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}