using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using MySql.Data.MySqlClient;
using HR_Applicant_System.Models;

namespace HR_Applicant_System.Views
{
    public partial class FinalReviewView : Window
    {
        public FinalReviewView()
        {
            InitializeComponent();
            SetupEvents();
            LoadPendingCandidates();
        }

        private void LoadPendingCandidates()
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = $@"SELECT a.ApplicationID, ap.FullName, j.JobTitle 
                                     FROM {DatabaseHelper.ApplicationTable} a
                                     JOIN {DatabaseHelper.ApplicantTable} ap ON a.ApplicantID = ap.ApplicantID
                                     JOIN {DatabaseHelper.JobTable} j ON a.VacancyID = j.VacancyID
                                     WHERE a.Status = 'For Final Review'";

                    using (var cmd = new MySqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        lstPendingCandidates.Items.Clear();
                        while (reader.Read())
                        {
                            var item = new ListBoxItem 
                            { 
                                Content = $"{reader["FullName"]} — {reader["JobTitle"]}", 
                                Tag = reader["ApplicationID"] 
                            };
                            lstPendingCandidates.Items.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex) 
            { 
                ShowMessage("Database Error: " + ex.Message); 
            }
        }

        private void SetupEvents()
        {
            if (btnReject != null) btnReject.Click += BtnReject_Click;
            if (btnApprove != null) btnApprove.Click += BtnApprove_Click;
        }

        private void BtnApprove_Click(object? sender, RoutedEventArgs e) => ExecuteDecision("Approved");
        private void BtnReject_Click(object? sender, RoutedEventArgs e) => ExecuteDecision("Rejected");

        private void ExecuteDecision(string status)
        {
            if (lstPendingCandidates.SelectedItem is ListBoxItem selected && selected.Tag is int appId)
            {
                // Fully qualified reference used here to resolve ambiguity
                var app = new HR_Applicant_System.Models.Application 
                { 
                    Id = appId, 
                    Status = status, 
                    HRRemarks = txtFinalRemarks.Text ?? "" 
                };
                
                new ApplicationRepository().UpdateApplicationStatus(app, app.HRRemarks);
                
                ShowMessage($"Status updated to {status}.");
                this.Close();
            }
            else 
            { 
                ShowMessage("Please select a candidate from the list first."); 
            }
        }

        private void ShowMessage(string message)
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                var dialog = new Window 
                { 
                    Width = 380, 
                    Height = 150, 
                    Title = "Notification", 
                    Content = new TextBlock 
                    { 
                        Text = message, 
                        Margin = new Thickness(20), 
                        TextWrapping = Avalonia.Media.TextWrapping.Wrap 
                    } 
                };
                // Used Show() instead of ShowDialog(this) to prevent crash on window close
                dialog.Show(); 
            });
        }
    }
}