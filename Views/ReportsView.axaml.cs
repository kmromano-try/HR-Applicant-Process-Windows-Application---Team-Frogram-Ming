﻿using System;
using System.Linq; 
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
                FontWeight = FontWeight.Bold,
                Foreground = Brushes.White
            };

            TextBlock subtitle = new TextBlock
            {
                Text = "Admin/Manager can view recruitment summaries and generate basic reports.",
                Foreground = new SolidColorBrush(Color.Parse("#e0e0e0")),
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
                Children = { title, subtitle, reportList, btnGenerate }
            };

            Border card = new Border
            {
                Background = new SolidColorBrush(Color.Parse("#252525")),
                CornerRadius = new CornerRadius(10),
                Padding = new Thickness(30),
                Margin = new Thickness(25),
                Child = contentPanel
            };

            Content = new Grid
            {
                Background = new SolidColorBrush(Color.Parse("#1e1e1e")),
                Children = { card }
            };
        }

        private void GenerateReport_Click(object? sender, RoutedEventArgs e)
        {
            if (reportList.SelectedItem == null)
            {
                ShowMessage("Please select a report first.");
                return;
            }

            string selectedReport = reportList.SelectedItem.ToString() ?? "";

            switch (selectedReport)
            {
                case "Applicant List Report":
                    ShowGenericReport("Applicant List Report", 
                        $"SELECT ApplicantID, FullName, Email FROM {DatabaseHelper.ApplicantTable}",
                        "ID: {0} | Name: {1} | Email: {2}", "ApplicantID", "FullName", "Email");
                    break;
                case "Active Job Vacancies Report":
                    ShowActiveJobVacanciesReport();
                    break;
                case "Accepted Applicants Report":
                    ShowGenericReport("Accepted Applicants Report",
                        $"SELECT a.ApplicationID, p.FullName FROM {DatabaseHelper.ApplicationTable} a JOIN {DatabaseHelper.ApplicantTable} p ON a.ApplicantID = p.ApplicantID WHERE a.Status = 'Accepted'",
                        "App ID: {0} | Applicant: {1}", "ApplicationID", "FullName");
                    break;
                case "Rejected Applicants Report":
                    ShowGenericReport("Rejected Applicants Report",
                        $"SELECT a.ApplicationID, p.FullName FROM {DatabaseHelper.ApplicationTable} a JOIN {DatabaseHelper.ApplicantTable} p ON a.ApplicantID = p.ApplicantID WHERE a.Status = 'Rejected'",
                        "App ID: {0} | Applicant: {1}", "ApplicationID", "FullName");
                    break;
            }
        }

        private void ShowActiveJobVacanciesReport()
        {
            ShowGenericReport("Active Job Vacancies Report",
                $"SELECT VacancyID, JobTitle, Department, Status FROM {DatabaseHelper.JobTable} WHERE Status = 'Active'",
                "Job #{0} | {1} | {2} | Status: {3}", "VacancyID", "JobTitle", "Department", "Status");
        }

        private void ShowGenericReport(string title, string query, string format, params string[] columns)
        {
            Window reportWindow = new Window { Width = 700, Height = 450, Title = title };
            ListBox list = new ListBox { Margin = new Thickness(20) };

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Fix for CS8619: Explicit cast to (object) ensures type compatibility
                            object[] values = columns.Select(col => (object)(reader[col]?.ToString() ?? "")).ToArray();
                            list.Items.Add(string.Format(format, values));
                        }
                    }
                }
                if (list.Items.Count == 0) list.Items.Add("No records found.");
            }
            catch (Exception ex) { list.Items.Add("Database error: " + ex.Message); }

            reportWindow.Content = new StackPanel { Children = { new TextBlock { Text = title, FontSize = 20, Margin = new Thickness(20) }, list } };
            reportWindow.Show();
        }

        private async void ShowMessage(string message)
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(async () =>
            {
                Window dialog = new Window { Width = 430, Height = 160, Title = "Message", Content = new TextBlock { Text = message, Margin = new Thickness(20), TextWrapping = TextWrapping.Wrap, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center } };
                await dialog.ShowDialog(this);
            });
        }
    }
}