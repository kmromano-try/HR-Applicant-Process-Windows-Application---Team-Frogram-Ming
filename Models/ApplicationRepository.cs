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
                string query = "SELECT applicant_name, target_position, current_status FROM applications";

                using (var cmd = new MySqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new ApplicantItem
                        {
                            Name = reader.IsDBNull(reader.GetOrdinal("applicant_name")) ? string.Empty : reader.GetString("applicant_name"),
                            Position = reader.IsDBNull(reader.GetOrdinal("target_position")) ? string.Empty : reader.GetString("target_position"),
                            Status = reader.IsDBNull(reader.GetOrdinal("current_status")) ? string.Empty : reader.GetString("current_status")
                        });
                    }
                }
            }
            return list;
        }
    }
}