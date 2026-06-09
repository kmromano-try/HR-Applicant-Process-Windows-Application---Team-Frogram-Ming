using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace HR_Applicant_System.Models
{
    public class JobRepository
    {
        public List<JobVacancy> GetAllJobs()
        {
            var list = new List<JobVacancy>();
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = "SELECT JobID, JobTitle, Department, VacancyStatus, CreatedAt FROM JobVacancies ORDER BY CreatedAt DESC";
                using (var cmd = new MySqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new JobVacancy
                        {
                            JobID = Convert.ToInt32(reader["JobID"]),
                            JobTitle = reader["JobTitle"].ToString() ?? string.Empty,
                            Department = reader["Department"].ToString() ?? string.Empty,
                            VacancyStatus = reader["VacancyStatus"].ToString() ?? string.Empty,
                            CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                        });
                    }
                }
            }
            return list;
        }

        public bool CreateJob(JobVacancy job)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"INSERT INTO JobVacancies (JobTitle, Department, JobDescription, VacancyStatus, CreatedAt) 
                                   VALUES (@Title, @Dept, @Desc, @Status, @Created)";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Title", job.JobTitle);
                        cmd.Parameters.AddWithValue("@Dept", job.Department);
                        cmd.Parameters.AddWithValue("@Desc", job.JobDescription);
                        cmd.Parameters.AddWithValue("@Status", job.VacancyStatus);
                        cmd.Parameters.AddWithValue("@Created", job.CreatedAt);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating job: {ex.Message}");
                return false;
            }
        }
    }
}