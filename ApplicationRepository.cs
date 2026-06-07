using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace HR_Applicant_System
{
    public class ApplicationRepository
    {
        public List<JobVacancySummary> GetJobsWithPendingApplications()
        {
            var list = new List<JobVacancySummary>();
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = @"SELECT DISTINCT j.JobVacancyID, j.JobTitle, j.Department 
                                 FROM JobVacancies j
                                 JOIN Applications a ON j.JobVacancyID = a.JobVacancyID
                                 WHERE a.Status = 'Submitted' OR a.Status = 'Reviewing'";
                using (var cmd = new MySqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new JobVacancySummary {
                            JobVacancyID = reader.GetInt32("JobVacancyID"),
                            JobTitle = reader.GetString("JobTitle"),
                            Department = reader.GetString("Department")
                        });
                    }
                }
            }
            return list;
        }

        public List<ApplicantDetailsSummary> GetApplicantsForJob(int jobVacancyId)
        {
            var list = new List<ApplicantDetailsSummary>();
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = @"SELECT a.ApplicationID, ap.FirstName, ap.LastName, ap.Email, a.SubmissionDate, a.HRRemarks
                                 FROM Applications a
                                 JOIN Applicants ap ON a.ApplicantID = ap.ApplicantID
                                 WHERE a.JobVacancyID = @JobVacancyID AND (a.Status = 'Submitted' OR a.Status = 'Reviewing')";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@JobVacancyID", jobVacancyId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new ApplicantDetailsSummary {
                                ApplicationID = reader.GetInt32("ApplicationID"),
                                ApplicantName = reader.GetString("FirstName") + " " + reader.GetString("LastName"),
                                Email = reader.GetString("Email"),
                                SubmissionDate = reader.GetDateTime("SubmissionDate"),
                                HRRemarks = reader.IsDBNull(reader.GetOrdinal("HRRemarks")) ? "" : reader.GetString("HRRemarks")
                            });
                        }
                    }
                }
            }
            return list;
        }

        public List<JobVacancySummary> GetJobsWithoutApplications()
        {
            var list = new List<JobVacancySummary>();
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = @"SELECT j.JobVacancyID, j.JobTitle, j.Department 
                                 FROM JobVacancies j
                                 LEFT JOIN Applications a ON j.JobVacancyID = a.JobVacancyID
                                 WHERE a.ApplicationID IS NULL";
                using (var cmd = new MySqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new JobVacancySummary {
                            JobVacancyID = reader.GetInt32("JobVacancyID"),
                            JobTitle = reader.GetString("JobTitle"),
                            Department = reader.GetString("Department")
                        });
                    }
                }
            }
            return list;
        }

        public List<ReviewedApplicationSummary> GetReviewedApplications()
        {
            var list = new List<ReviewedApplicationSummary>();
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = @"SELECT a.ApplicationID, ap.FirstName, ap.LastName, j.JobTitle, a.Status, h.ChangeDate
                                 FROM Applications a
                                 JOIN Applicants ap ON a.ApplicantID = ap.ApplicantID
                                 JOIN JobVacancies j ON a.JobVacancyID = j.JobVacancyID
                                 LEFT JOIN ApplicationStatusHistory h ON a.ApplicationID = h.ApplicationID
                                 WHERE a.Status = 'Staff Approved' OR a.Status = 'Staff Rejected'
                                 ORDER BY h.ChangeDate DESC";
                using (var cmd = new MySqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new ReviewedApplicationSummary {
                            ApplicationID = reader.GetInt32("ApplicationID"),
                            ApplicantName = reader.GetString("FirstName") + " " + reader.GetString("LastName"),
                            JobTitle = reader.GetString("JobTitle"),
                            StaffDecision = reader.GetString("Status"),
                            DateReviewed = reader.IsDBNull(reader.GetOrdinal("ChangeDate")) ? DateTime.Now : reader.GetDateTime("ChangeDate")
                        });
                    }
                }
            }
            return list;
        }

        public bool ReviewApplication(int applicationId, string newStatus, string hrRemarks, string staffName)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        string updateSql = "UPDATE Applications SET Status = @Status, HRRemarks = @Remarks WHERE ApplicationID = @AppID";
                        using (var cmd = new MySqlCommand(updateSql, conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@Status", newStatus);
                            cmd.Parameters.AddWithValue("@Remarks", hrRemarks);
                            cmd.Parameters.AddWithValue("@AppID", applicationId);
                            cmd.ExecuteNonQuery();
                        }

                        string historySql = "INSERT INTO ApplicationStatusHistory (ApplicationID, StatusChangedTo, ChangeDate, ChangedBy) VALUES (@AppID, @Status, @CDate, @CBy)";
                        using (var cmd = new MySqlCommand(historySql, conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@AppID", applicationId);
                            cmd.Parameters.AddWithValue("@Status", newStatus);
                            cmd.Parameters.AddWithValue("@CDate", DateTime.Now);
                            cmd.Parameters.AddWithValue("@CBy", staffName);
                            cmd.ExecuteNonQuery();
                        }

                        trans.Commit();
                        return true;
                    }
                    catch
                    {
                        trans.Rollback();
                        return false;
                    }
                }
            }
        }
    }

    public class JobVacancySummary { public int JobVacancyID { get; set; } public string JobTitle { get; set; } public string Department { get; set; } }
    public class ApplicantDetailsSummary { public int ApplicationID { get; set; } public string ApplicantName { get; set; } public string Email { get; set; } public DateTime SubmissionDate { get; set; } public string HRRemarks { get; set; } }
    public class ReviewedApplicationSummary { public int ApplicationID { get; set; } public string ApplicantName { get; set; } public string JobTitle { get; set; } public string StaffDecision { get; set; } public DateTime DateReviewed { get; set; } }
}