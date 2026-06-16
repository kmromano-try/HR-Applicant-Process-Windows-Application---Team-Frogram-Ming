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
                // Define the query string for GetAllJobs
                string query = "SELECT VacancyID, JobTitle, Department, JobDescription, MinimumQualifications, VacancyStatus FROM JobVacancies";
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
                            Qualifications = reader["MinimumQualifications"].ToString() ?? string.Empty,
                            Status = reader["VacancyStatus"].ToString() ?? string.Empty // Map VacancyStatus from DB to Status in model
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
                    string query = @"INSERT INTO JobVacancies (JobTitle, Department, JobDescription, MinimumQualifications, VacancyStatus) 
                                   VALUES (@Title, @Dept, @Desc, @Quals, @Status)";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Title", job.JobTitle);
                        cmd.Parameters.AddWithValue("@Dept", job.Department);
                        cmd.Parameters.AddWithValue("@Desc", job.JobDescription);
                        cmd.Parameters.AddWithValue("@Quals", job.Qualifications); // Assign Qualifications
                        cmd.Parameters.AddWithValue("@Status", job.Status); // Use Status from model
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