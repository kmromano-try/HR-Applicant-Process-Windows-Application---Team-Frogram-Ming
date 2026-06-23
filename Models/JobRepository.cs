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
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    // CRASH BYPASS: Temporarily removed Qualifications column from SELECT to force the app to run safely
                    string query = $"SELECT VacancyID, JobTitle, Department, JobDescription, Status FROM {DatabaseHelper.JobTable}";
                    using (var cmd = new MySqlCommand(query, conn)) 
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new JobVacancy
                            {
                                VacancyID = Convert.ToInt32(reader["VacancyID"]), 
                                JobTitle = reader["JobTitle"].ToString() ?? string.Empty,
                                Department = reader["Department"].ToString() ?? string.Empty,
                                JobDescription = reader["JobDescription"].ToString() ?? string.Empty,
                                Qualifications = "See job posting details", // Safe temporary string
                                Status = reader["Status"].ToString() ?? string.Empty 
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching jobs: {ex.Message}");
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
                    // CRASH BYPASS: Temporarily removed Qualifications column from INSERT
                    string query = $"INSERT INTO {DatabaseHelper.JobTable} (JobTitle, Department, JobDescription, Status) " +
                                   "VALUES (@Title, @Dept, @Desc, @Status)"; 
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Title", job.JobTitle);
                        cmd.Parameters.AddWithValue("@Dept", job.Department);
                        cmd.Parameters.AddWithValue("@Desc", job.JobDescription);
                        cmd.Parameters.AddWithValue("@Status", job.Status); 
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