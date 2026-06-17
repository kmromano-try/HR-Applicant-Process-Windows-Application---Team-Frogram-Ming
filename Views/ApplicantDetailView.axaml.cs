using Avalonia;
using Avalonia.Controls;
using MySql.Data.MySqlClient;
using HR_Applicant_System.Models;
using System;

namespace HR_Applicant_System.Views
{
    public partial class ApplicantDetailView : Window
    {
        public ApplicantDetailView(int applicationId)
        {
            InitializeComponent();
            LoadData(applicationId);
        }

        private void LoadData(int applicationId)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    // FIXED: Changed BioDescription to Bio to match your database
                    string query = $"SELECT FullName, Bio FROM {DatabaseHelper.ApplicantTable} WHERE ApplicantID = @ID";
                    
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ID", applicationId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // If you have controls named txtName and txtBio in your .axaml, uncomment these:
                                // this.FindControl<TextBlock>("txtName").Text = reader["FullName"].ToString();
                                // this.FindControl<TextBlock>("txtBio").Text = reader["Bio"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }
}