using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
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

                    string activeJobsQuery =
                        $"SELECT COUNT(*) FROM {DatabaseHelper.JobTable} WHERE Status = 'Active'";

                    using (MySqlCommand cmd = new MySqlCommand(activeJobsQuery, conn))
                    {
                        int activeJobs = Convert.ToInt32(cmd.ExecuteScalar());

                        if (activeJobsCountBlock != null)
                            activeJobsCountBlock.Text = activeJobs.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Active Jobs counter error: " + ex.Message);

                if (activeJobsCountBlock != null)
                    activeJobsCountBlock.Text = "0";
            }

            try
            {
                using (MySqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    string finalReviewQuery =
                        $"SELECT COUNT(*) FROM {DatabaseHelper.ApplicationTable} WHERE Status = 'For Final Review'";

                    using (MySqlCommand cmd = new MySqlCommand(finalReviewQuery, conn))
                    {
                        int finalReview = Convert.ToInt32(cmd.ExecuteScalar());

                        if (finalReviewCountBlock != null)
                            finalReviewCountBlock.Text = finalReview.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Final Review counter error: " + ex.Message);

                if (finalReviewCountBlock != null)
                    finalReviewCountBlock.Text = "0";
            }

            try
            {
                using (MySqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    string rejectedQuery =
                        $"SELECT COUNT(*) FROM {DatabaseHelper.ApplicationTable} WHERE Status = 'Rejected'";

                    using (MySqlCommand cmd = new MySqlCommand(rejectedQuery, conn))
                    {
                        int rejected = Convert.ToInt32(cmd.ExecuteScalar());

                        if (rejectedCountBlock != null)
                            rejectedCountBlock.Text = rejected.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Rejected queue counter error: " + ex.Message);

                if (rejectedCountBlock != null)
                    rejectedCountBlock.Text = "0";
            }
        }

        public void Dashboard_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
                LoadDashboardCounts();
            }
            catch (Exception ex)
            {
                ShowMessage("Failed to sync dashboard data: " + ex.Message);
            }
        }

        public void CreateStaff_Click(object? sender, RoutedEventArgs e)
        {
            var createStaffView = new CreateStaffView();

            createStaffView.Closed += (s, args) =>
            {
                LoadDashboardCounts();
            };

            createStaffView.Show();
        }

        public void CreateJob_Click(object? sender, RoutedEventArgs e)
        {
            var createJobView = new CreateJobView();

            createJobView.Closed += (s, args) =>
            {
                LoadDashboardCounts();
            };

            createJobView.Show();
        }

        public void FinalReview_Click(object? sender, RoutedEventArgs e)
        {
            var finalReviewView = new FinalReviewView();

            finalReviewView.Closed += (s, args) =>
            {
                LoadDashboardCounts();
            };

            finalReviewView.Show();
        }

        public void RejectedQueue_Click(object? sender, RoutedEventArgs e)
        {
            var rejectedQueueView = new RejectedQueueView();

            rejectedQueueView.Closed += (s, args) =>
            {
                LoadDashboardCounts();
            };

            rejectedQueueView.Show();
        }

        public void Reports_Click(object? sender, RoutedEventArgs e)
        {
            var reportsView = new ReportsView();
            reportsView.Show();
        }

        public void Logout_Click(object? sender, RoutedEventArgs e)
        {
            var staffLoginView = new StaffLoginView();
            staffLoginView.Show();
            Close();
        }

        private void ShowMessage(string message)
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(async () =>
            {
                Window dialog = new Window
                {
                    Width = 420,
                    Height = 160,
                    Title = "Message",
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    Content = new TextBlock
                    {
                        Text = message,
                        Margin = new Avalonia.Thickness(20),
                        TextWrapping = TextWrapping.Wrap,
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center
                    }
                };

                await dialog.ShowDialog(this);
            });
        }
    }
}