using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using System;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using HR_Applicant_System.Models;

namespace HR_Applicant_System.Views
{
    public class ApplicantDetailView : Window
    {
        private TextBlock _txtName;
        private TextBlock _txtBio;

        public ApplicantDetailView(int applicationId)
        {
            Width = 500;
            Height = 400;
            Title = "Applicant Profile Details";
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            Background = new SolidColorBrush(Color.Parse("#1e1e1e"));

            _txtName = new TextBlock
            {
                FontSize = 24,
                FontWeight = FontWeight.Bold,
                Foreground = Brushes.White,
                Margin = new Thickness(0, 0, 0, 10)
            };

            _txtBio = new TextBlock
            {
                FontSize = 16,
                Foreground = new SolidColorBrush(Color.Parse("#e0e0e0")),
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = VerticalAlignment.Top
            };

            StackPanel layout = new StackPanel
            {
                Margin = new Thickness(30),
                Spacing = 10,
                Children =
                {
                    new TextBlock { Text = "APPLICANT NAME", FontSize = 12, Foreground = Brushes.Gray, FontWeight = FontWeight.SemiBold },
                    _txtName,
                    new Separator { Background = Brushes.Gray, Opacity = 0.3, Margin = new Thickness(0, 10) },
                    new TextBlock { Text = "BIOGRAPHY / DESCRIPTION", FontSize = 12, Foreground = Brushes.Gray, FontWeight = FontWeight.SemiBold },
                    new ScrollViewer 
                    { 
                        Content = _txtBio, 
                        Height = 200, 
                        VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto 
                    }
                }
            };

            Content = layout;

            // Load data asynchronously on initialization
            Task.Run(() => LoadApplicantBio(applicationId));
        }

        private void LoadApplicantBio(int id)
        {
            try
            {
                using (MySqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    // INNER JOIN matching ApplicationID to fetch associated Applicant details
                    string query = $@"
                        SELECT ap.FullName, ap.BioDescription 
                        FROM {DatabaseHelper.ApplicationTable} a
                        INNER JOIN {DatabaseHelper.ApplicantTable} ap ON a.ApplicantID = ap.ApplicantID
                        WHERE a.ApplicationID = @id";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string name = reader["FullName"]?.ToString() ?? "Unknown";
                                string bio = reader.IsDBNull(reader.GetOrdinal("BioDescription"))
                                    ? "No biography details provided."
                                    : (reader["BioDescription"]?.ToString() ?? "No biography details provided.");
                                
                                if (string.IsNullOrWhiteSpace(bio)) bio = "No biography details provided.";

                                // Marshal back to UI thread for property updates
                                Dispatcher.UIThread.Post(() =>
                                {
                                    _txtName.Text = name;
                                    _txtBio.Text = bio;
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Dispatcher.UIThread.Post(() => {
                    _txtBio.Text = "Error loading details: " + ex.Message;
                });
            }
        }
    }
}