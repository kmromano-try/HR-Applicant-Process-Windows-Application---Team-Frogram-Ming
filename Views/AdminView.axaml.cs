using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MySql.Data.MySqlClient;
using HR_Applicant_System.Models;

namespace HR_Applicant_System.Views
{
    public partial class AdminView : Window
    {
        private TextBlock? activeJobsCountBlock;
        private TextBlock? finalReviewCountBlock;
        private TextBlock? rejectedCountBlock;

        public AdminView()
        {
            InitializeComponent();

            activeJobsCountBlock = this.FindControl<TextBlock>("txtActiveJobsCount");
            finalReviewCountBlock = this.FindControl<TextBlock>("txtFinalReviewCount");
            rejectedCountBlock = this.FindControl<TextBlock>("txtRejectedCount");

            LoadDashboardCounts();
        }

        private void LoadDashboardCounts()
        {
            try
            {
                using (MySqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    // Count active job vacancies
                    string activeJobsQuery = "SELECT COUNT(*) FROM JobVacancies WHERE VacancyStatus = 'Active'";
                    using (MySqlCommand cmd = new MySqlCommand(activeJobsQuery, conn))
                    {
                        int activeJobs = Convert.ToInt32(cmd.ExecuteScalar());

                        if (activeJobsCountBlock != null)
                        {
                            activeJobsCountBlock.Text = activeJobs.ToString();
                        }
                    }

                    // Count applications waiting for Admin/Manager final decision
                    string finalReviewQuery = "SELECT COUNT(*) FROM Applications WHERE CurrentStatus = 'For Final Review'";
                    using (MySqlCommand cmd = new MySqlCommand(finalReviewQuery, conn))
                    {
                        int finalReview = Convert.ToInt32(cmd.ExecuteScalar());

                        if (finalReviewCountBlock != null)
                        {
                            finalReviewCountBlock.Text = finalReview.ToString();
                        }
                    }

                    // Count applicants rejected by HR Staff only
                    // Final rejected applicants by Admin/Manager are not included here
                    string rejectedQuery = @"
                        SELECT COUNT(*) 
                        FROM Applications a
                        WHERE a.CurrentStatus = 'Rejected'
                        AND NOT EXISTS (
                            SELECT 1 
                            FROM HiringDecisions h
                            WHERE h.ApplicationID = a.ApplicationID
                        )";

                    using (MySqlCommand cmd = new MySqlCommand(rejectedQuery, conn))
                    {
                        int rejected = Convert.ToInt32(cmd.ExecuteScalar());

                        if (rejectedCountBlock != null)
                        {
                            rejectedCountBlock.Text = rejected.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Dashboard count error: " + ex.Message);
            }
        }

        private void Dashboard_Click(object? sender, RoutedEventArgs e)
        {
            LoadDashboardCounts();
        }

        private void CreateStaff_Click(object? sender, RoutedEventArgs e)
        {
            CreateStaffView createStaffView = new CreateStaffView();

            createStaffView.Closed += (s, args) =>
            {
                LoadDashboardCounts();
            };

            createStaffView.Show();
        }

        private void CreateJob_Click(object? sender, RoutedEventArgs e)
        {
            CreateJobView createJobView = new CreateJobView();

            createJobView.Closed += (s, args) =>
            {
                LoadDashboardCounts();
            };

            createJobView.Show();
        }

        private void FinalReview_Click(object? sender, RoutedEventArgs e)
        {
            FinalReviewView finalReviewView = new FinalReviewView();

            finalReviewView.Closed += (s, args) =>
            {
                LoadDashboardCounts();
            };

            finalReviewView.Show();
        }

        private void RejectedQueue_Click(object? sender, RoutedEventArgs e)
        {
            RejectedQueueView rejectedQueueView = new RejectedQueueView();

            rejectedQueueView.Closed += (s, args) =>
            {
                LoadDashboardCounts();
            };

            rejectedQueueView.Show();
        }

        private void Reports_Click(object? sender, RoutedEventArgs e)
        {
            ReportsView reportsView = new ReportsView();
            reportsView.Show();
        }

        private void Logout_Click(object? sender, RoutedEventArgs e)
        {
            HR_Applicant_System.MainWindow mainWindow = new HR_Applicant_System.MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}