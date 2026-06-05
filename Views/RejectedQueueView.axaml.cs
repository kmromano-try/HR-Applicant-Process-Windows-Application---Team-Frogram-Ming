using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using MySql.Data.MySqlClient;
using HR_Applicant_System.Models;

namespace HR_Applicant_System.Views
{
    public class RejectedQueueView : Window
    {
        private ListBox rejectedList;

        public RejectedQueueView()
        {
            Width = 800;
            Height = 520;
            Title = "Staff Rejected Queue";

            TextBlock title = new TextBlock
            {
                Text = "Staff Rejected Queue",
                FontSize = 26,
                FontWeight = FontWeight.Bold
            };

            TextBlock subtitle = new TextBlock
            {
                Text = "This section shows applicants rejected by HR Staff during initial screening. It serves as a read-only audit log for Admin/Manager transparency.",
                Foreground = Brushes.Gray,
                TextWrapping = TextWrapping.Wrap
            };

            rejectedList = new ListBox
            {
                Height = 300
            };

            TextBlock note = new TextBlock
            {
                Text = "Note: This queue is for review and monitoring only. No final action is required here.",
                Foreground = Brushes.DarkRed,
                TextWrapping = TextWrapping.Wrap
            };

            StackPanel contentPanel = new StackPanel
            {
                Spacing = 15,
                Children =
                {
                    title,
                    subtitle,
                    rejectedList,
                    note
                }
            };

            Border card = new Border
            {
                Background = Brushes.White,
                CornerRadius = new CornerRadius(10),
                Padding = new Thickness(30),
                Margin = new Thickness(25),
                Child = contentPanel
            };

            Grid mainGrid = new Grid
            {
                Background = new SolidColorBrush(Color.Parse("#F3F4F6")),
                Children =
                {
                    card
                }
            };

            Content = mainGrid;

            LoadRejectedApplicants();
        }

        private void LoadRejectedApplicants()
        {
            rejectedList.Items.Clear();

            try
            {
                using (MySqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    string query = @"
                        SELECT 
                            a.ApplicationID,
                            ap.FullName,
                            j.JobTitle,
                            a.StaffReviewer,
                            a.ScreeningRemarks,
                            a.CurrentStatus
                        FROM Applications a
                        INNER JOIN Applicants ap ON a.ApplicantID = ap.ApplicantID
                        INNER JOIN JobVacancies j ON a.JobID = j.JobID
                        WHERE a.CurrentStatus = 'Rejected'
                        AND NOT EXISTS (
                            SELECT 1 
                            FROM HiringDecisions h
                            WHERE h.ApplicationID = a.ApplicationID
                        )";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string item =
                                "Application #" + reader["ApplicationID"] +
                                " | " + reader["FullName"] +
                                " | " + reader["JobTitle"] +
                                " | Reviewed by: " + reader["StaffReviewer"] +
                                " | Reason: " + reader["ScreeningRemarks"];

                            rejectedList.Items.Add(item);
                        }
                    }
                }

                if (rejectedList.Items.Count == 0)
                {
                    rejectedList.Items.Add("No staff-rejected applicants found.");
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Database error: " + ex.Message);
            }
        }

        private async void ShowMessage(string message)
        {
            Window dialog = new Window
            {
                Width = 420,
                Height = 160,
                Title = "Message",
                Content = new TextBlock
                {
                    Text = message,
                    Margin = new Thickness(20),
                    TextWrapping = TextWrapping.Wrap,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                }
            };

            await dialog.ShowDialog(this);
        }
    }
}