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
                string query = $@"SELECT a.ApplicationID, ap.FullName, ap.Email, ap.ContactNumber, ap.Bio, 
                                  ap.Experience, ap.ResumeFilePath, j.JobTitle, a.Status, a.StaffFeedback
                                  FROM {DatabaseHelper.ApplicationTable} a
                                  INNER JOIN {DatabaseHelper.ApplicantTable} ap ON a.ApplicantID = ap.ApplicantID
                                  INNER JOIN {DatabaseHelper.JobTable} j ON a.VacancyID = j.VacancyID";
                
                using (var cmd = new MySqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        applications.Add(new Application
                        {
                            Id = reader.GetInt32("ApplicationID"),
                            Name = reader.IsDBNull(reader.GetOrdinal("FullName")) ? "Unknown" : reader.GetString("FullName"),
                            Position = reader.IsDBNull(reader.GetOrdinal("JobTitle")) ? "Unassigned" : reader.GetString("JobTitle"),
                            Status = reader.IsDBNull(reader.GetOrdinal("Status")) ? "Pending" : reader.GetString("Status"),
                            HRRemarks = reader.IsDBNull(reader.GetOrdinal("StaffFeedback")) ? string.Empty : reader.GetString("StaffFeedback"),
                            // Mapping these fields so they are no longer invisible in the UI
                            Bio = reader.IsDBNull(reader.GetOrdinal("Bio")) ? "No bio provided" : reader.GetString("Bio"),
                            Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? string.Empty : reader.GetString("Email"),
                            ContactNumber = reader.IsDBNull(reader.GetOrdinal("ContactNumber")) ? string.Empty : reader.GetString("ContactNumber")
                        });
                    }
                }
            }
            return applications;
        }

        public void UpdateApplicationStatus(Application application, string loggingRemarks)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                
                string oldStatus = "Pending";
                using (var selectCmd = new MySqlCommand($"SELECT Status FROM {DatabaseHelper.ApplicationTable} WHERE ApplicationID = @id", conn))
                {
                    selectCmd.Parameters.AddWithValue("@id", application.Id);
                    object? res = selectCmd.ExecuteScalar();
                    if (res != null) oldStatus = res.ToString() ?? "Pending";
                }

                if (oldStatus == application.Status) return;

                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string updateQuery = $"UPDATE {DatabaseHelper.ApplicationTable} SET Status = @status, StaffFeedback = @hrRemarks WHERE ApplicationID = @id";
                        using (var updateCmd = new MySqlCommand(updateQuery, conn, transaction))
                        {
                            updateCmd.Parameters.AddWithValue("@status", application.Status);
                            updateCmd.Parameters.AddWithValue("@hrRemarks", application.HRRemarks);
                            updateCmd.Parameters.AddWithValue("@id", application.Id);
                            updateCmd.ExecuteNonQuery();
                        }

                        string historyQuery = @"INSERT INTO ApplicationStatusHistory (ApplicationID, OldStatus, NewStatus, Remarks, ChangeDate, ChangedByUserId) 
                                               VALUES (@appId, @old, @new, @rem, @date, 0)";
                        using (var historyCmd = new MySqlCommand(historyQuery, conn, transaction))
                        {
                            historyCmd.Parameters.AddWithValue("@appId", application.Id);
                            historyCmd.Parameters.AddWithValue("@old", oldStatus);
                            historyCmd.Parameters.AddWithValue("@new", application.Status);
                            historyCmd.Parameters.AddWithValue("@rem", loggingRemarks);
                            historyCmd.Parameters.AddWithValue("@date", DateTime.Now);
                            historyCmd.ExecuteNonQuery();
                        }
                        transaction.Commit();
                    }
                    catch 
                    { 
                        transaction.Rollback(); 
                        throw; 
                    }
                }
            }
        }
    }
}