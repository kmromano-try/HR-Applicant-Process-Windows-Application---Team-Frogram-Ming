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
    public class ReportsView : Window
    {
        private ListBox reportList;

        public ReportsView()
        {
            Width = 750;
            Height = 520;
            Title = "Reports";

            TextBlock title = new TextBlock
            {
                Text = "Recruitment Reports",
                FontSize = 26,
                FontWeight = FontWeight.Bold
            };

            TextBlock subtitle = new TextBlock
            {
                Text = "Admin/Manager can view recruitment summaries and generate basic reports.",
                Foreground = Brushes.Gray,
                TextWrapping = TextWrapping.Wrap
            };

            reportList = new ListBox
            {
                Height = 250,
                Items =
                {
                    "Applicant List Report",
                    "Active Job Vacancies Report",
                    "Accepted Applicants Report",
                    "Rejected Applicants Report"
                }
            };

            Button btnGenerate = new Button
            {
                Content = "Generate Selected Report",
                Height = 42,
                Width = 220
            };

            btnGenerate.Click += GenerateReport_Click;

            StackPanel contentPanel = new StackPanel
            {
                Spacing = 15,
                Children =
                {
                    title,
                    subtitle,
                    reportList,
                    btnGenerate
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
        }

        private void GenerateReport_Click(object? sender, RoutedEventArgs e)
        {
            if (reportList.SelectedItem == null)
            {
                ShowMessage("Please select a report first.");
                return;
            }

            string selectedReport = reportList.SelectedItem.ToString() ?? "";

            if (selectedReport == "Active Job Vacancies Report")
            {
                ShowActiveJobVacanciesReport();
            }
            else
            {
                ShowMessage("This report option is listed for the full system. Please select Active Job Vacancies Report for the working report demo.");
            }
        }

        private void ShowActiveJobVacanciesReport()
        {
            Window reportWindow = new Window
            {
                Width = 800,
                Height = 500,
                Title = "Active Job Vacancies Report"
            };

            ListBox jobList = new ListBox
            {
                Height = 340
            };

            try
            {
                using (MySqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    string query = @"
                        SELECT JobID, JobTitle, Department, VacancyStatus, CreatedAt
                        FROM JobVacancies
                        WHERE VacancyStatus = 'Active'
                        ORDER BY CreatedAt DESC";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string item =
                                "Job #" + reader["JobID"] +
                                " | " + reader["JobTitle"] +
                                " | " + reader["Department"] +
                                " | Status: " + reader["VacancyStatus"] +
                                " | Posted: " + Convert.ToDateTime(reader["CreatedAt"]).ToString("MMM dd, yyyy");

                            jobList.Items.Add(item);
                        }
                    }
                }

                if (jobList.Items.Count == 0)
                {
                    jobList.Items.Add("No active job vacancies found.");
                }
            }
            catch (Exception ex)
            {
                jobList.Items.Add("Database error: " + ex.Message);
            }

            StackPanel panel = new StackPanel
            {
                Margin = new Thickness(25),
                Spacing = 15,
                Children =
                {
                    new TextBlock
                    {
                        Text = "Active Job Vacancies Report",
                        FontSize = 24,
                        FontWeight = FontWeight.Bold
                    },
                    new TextBlock
                    {
                        Text = "This report shows all job vacancies currently marked as Active.",
                        Foreground = Brushes.Gray,
                        TextWrapping = TextWrapping.Wrap
                    },
                    jobList
                }
            };

            reportWindow.Content = panel;
            reportWindow.Show();
        }

        private async void ShowMessage(string message)
        {
            Window dialog = new Window
            {
                Width = 430,
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