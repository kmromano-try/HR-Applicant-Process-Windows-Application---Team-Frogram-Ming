using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace HR_Applicant_System.Models
{
    public class ApplicantItem
    {
        public int ApplicationID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    public class ApplicationRepository
    {
        public List<ApplicantItem> GetAllActiveApplications()
        {
            var list = new List<ApplicantItem>();
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = "SELECT application_id, name, position, status FROM applications WHERE status != 'Rejected' AND status != 'For Final Review'";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new ApplicantItem
                            {
                                ApplicationID = Convert.ToInt32(reader["application_id"]),
                                Name = reader["name"]?.ToString() ?? string.Empty,
                                Position = reader["position"]?.ToString() ?? string.Empty,
                                Status = reader["status"]?.ToString() ?? string.Empty
                            });
                        }
                    }
                }
            }
            return list;
        }

        public List<ApplicantItem> GetFinalReviewApplications()
        {
            var list = new List<ApplicantItem>();
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = "SELECT application_id, name, position, status FROM applications WHERE status = 'For Final Review'";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new ApplicantItem
                            {
                                ApplicationID = Convert.ToInt32(reader["application_id"]),
                                Name = reader["name"]?.ToString() ?? string.Empty,
                                Position = reader["position"]?.ToString() ?? string.Empty,
                                Status = reader["status"]?.ToString() ?? string.Empty
                            });
                        }
                    }
                }
            }
            return list;
        }

        public bool UpdateApplicationStatus(int applicantId, string status)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = "UPDATE applications SET status = @status WHERE application_id = @applicantId";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@status", status);
                    cmd.Parameters.AddWithValue("@applicantId", applicantId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool SaveInterviewSchedule(int applicantId, string interviewer, string date, string time, string remarks)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = "INSERT INTO interviews (applicant_id, interviewer, interview_date, interview_time, hr_remarks) VALUES (@applicantId, @interviewer, @date, @time, @remarks)";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@applicantId", applicantId);
                    cmd.Parameters.AddWithValue("@interviewer", interviewer);
                    cmd.Parameters.AddWithValue("@date", date);
                    cmd.Parameters.AddWithValue("@time", time);
                    cmd.Parameters.AddWithValue("@remarks", remarks);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}