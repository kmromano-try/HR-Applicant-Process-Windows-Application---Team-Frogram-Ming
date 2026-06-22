using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace HR_Applicant_System.Models
{
    public class ApplicationRepository
    {
        private readonly string _connectionString = "Server=localhost;Database=hr_capstone_db;Uid=root;Pwd=;";

        public List<Application> GetAllActiveApplications()
        {
            var applications = new List<Application>();

            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                var query = "SELECT id, name, position, status, hr_remarks FROM applications";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            applications.Add(new Application
                            {
                                Id = reader.GetInt32("id"),
                                Name = reader.GetString("name"),
                                Position = reader.GetString("position"),
                                Status = reader.GetString("status"),
                                HRRemarks = reader.IsDBNull(reader.GetOrdinal("hr_remarks")) ? string.Empty : reader.GetString("hr_remarks")
                            });
                        }
                    }
                }
            }

            return applications;
        }

        public void UpdateApplicationStatus(Application application)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                var query = "UPDATE applications SET status = @status, hr_remarks = @hrRemarks WHERE id = @id";
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