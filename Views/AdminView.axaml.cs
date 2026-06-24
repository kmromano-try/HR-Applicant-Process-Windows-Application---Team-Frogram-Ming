using System;
using Avalonia;
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

            // Linking the C# fields to the XAML x:Name components
            activeJobsCountBlock = this.FindControl<TextBlock>("txtActiveJobsCount");
            finalReviewCountBlock = this.FindControl<TextBlock>("txtFinalReviewCount");
            rejectedCountBlock = this.FindControl<TextBlock>("txtRejectedCount");

            LoadDashboardCounts();
        }

        private void LoadDashboardCounts()
        {
            // 1. Count active job vacancies
            try
            {
                using (MySqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string activeJobsQuery = $"SELECT COUNT(*) FROM {DatabaseHelper.JobTable} WHERE Status = 'Active'";
                    using (MySqlCommand cmd = new MySqlCommand(activeJobsQuery, conn))
                    {
                        int activeJobs = Convert.ToInt32(cmd.ExecuteScalar());

                        if (activeJobsCountBlock != null)
                        {
                            activeJobsCountBlock.Text = activeJobs.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Active Jobs counter error: " + ex.Message);
                if (activeJobsCountBlock != null) activeJobsCountBlock.Text = "0";
            }

            // 2. Count applications waiting for Admin/Manager final decision
            try
            {
                using (MySqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string finalReviewQuery = $"SELECT COUNT(*) FROM {DatabaseHelper.ApplicationTable} WHERE Status = 'For Final Review'";
                    using (MySqlCommand cmd = new MySqlCommand(finalReviewQuery, conn))
                    {
                        int finalReview = Convert.ToInt32(cmd.ExecuteScalar());

                        if (finalReviewCountBlock != null)
                        {
                            finalReviewCountBlock.Text = finalReview.ToString();
                        }
                    }
                }
            }
            catch (Exception ex) 
            {
                Console.WriteLine("Final Review counter error: " + ex.Message);
                if (finalReviewCountBlock != null) finalReviewCountBlock.Text = "0";
            }

            // 3. Count applicants rejected by HR Staff
            try
            {
                using (MySqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string rejectedQuery = $"SELECT COUNT(*) FROM {DatabaseHelper.ApplicationTable} WHERE Status = 'Rejected'";

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
                Console.WriteLine("Rejected queue counter error: " + ex.Message);
                if (rejectedCountBlock != null) rejectedCountBlock.Text = "0";
            }
        }

        public void DeleteJob_Click(object? sender, RoutedEventArgs e)
        {
            var txtInput = new TextBox { PlaceholderText = "Enter Numeric ID (e.g., 3)" };
            var btnConfirm = new Button 
            { 
                Content = "Permanently Remove Vacancy Track", 
                Background = new SolidColorBrush(Color.Parse("#ef4444")), 
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = new Avalonia.Thickness(0, 10, 0, 0)
            };

            Window dialog = new Window
            {
                Width = 380,
                Height = 180,
                Title = "System Subsystem: Delete Vacancy",
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Background = new SolidColorBrush(Color.Parse("#252525")),
                Padding = new Avalonia.Thickness(20),
                Content = new StackPanel
                {
                    Spacing = 8,
                    Children =
                    {
                        new TextBlock { Text = "Target Vacancy Reference ID:", Foreground = Brushes.White, FontWeight = FontWeight.Bold },
                        txtInput,
                        btnConfirm
                    }
                }
            };

            btnConfirm.Click += (s, args) =>
            {
                string rawInput = txtInput.Text ?? "";
                if (int.TryParse(rawInput, out int targetId))
                {
                    var repo = new JobRepository();
                    if (repo.DeleteJob(targetId))
                    {
                        dialog.Close();
                        ShowMessage($"Success: Vacancy #{targetId} cleared from system profile listings.");
                        LoadDashboardCounts();
                    }
                    else
                    {
                        ShowMessage("Execution Blocker: Target ID missing or database constraints rejected operations.");
                    }
                }
                else
                {
                    ShowMessage("Format Error: Ensure tracking parameters passed are clean whole integer digits.");
                }
            };

            dialog.ShowDialog(this);
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
            CreateStaffView createStaffView = new CreateStaffView();

            createStaffView.Closed += (s, args) =>
            {
                LoadDashboardCounts();
            };

            createStaffView.Show();
        }

        public void CreateJob_Click(object? sender, RoutedEventArgs e)
        {
            CreateJobView createJobView = new CreateJobView();

            createJobView.Closed += (s, args) =>
            {
                LoadDashboardCounts();
            };

            createJobView.Show();
        }

        public void FinalReview_Click(object? sender, RoutedEventArgs e)
        {
            FinalReviewView finalReviewView = new FinalReviewView();

            finalReviewView.Closed += (s, args) =>
            {
                LoadDashboardCounts();
            };

            finalReviewView.Show();
        }

        public void RejectedQueue_Click(object? sender, RoutedEventArgs e)
        {
            RejectedQueueView rejectedQueueView = new RejectedQueueView();

            rejectedQueueView.Closed += (s, args) =>
            {
                LoadDashboardCounts();
            };

            rejectedQueueView.Show();
        }

        public void Reports_Click(object? sender, RoutedEventArgs e)
        {
            ReportsView reportsView = new ReportsView();
            reportsView.Show();
        }

        public void Logout_Click(object? sender, RoutedEventArgs e)
        {
            HR_Applicant_System.MainWindow mainWindow = new HR_Applicant_System.MainWindow();
            mainWindow.Show();
            this.Close();
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