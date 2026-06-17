﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using System;
using MySql.Data.MySqlClient;
using HR_Applicant_System.Models;
namespace HR_Applicant_System.Views
{
    public class CreateJobView : Window
    {
        private TextBox txtJobTitle;
        private TextBox txtDepartment;
        private TextBox txtDescription;
        private TextBox txtQualifications;

        public CreateJobView()
        {
            Width = 550;
            Height = 520;
            Title = "Create Job Vacancy";

            txtJobTitle = new TextBox
            {
                PlaceholderText = "Enter job title"
            };

            txtDepartment = new TextBox
            {
                PlaceholderText = "Enter department"
            };

            txtDescription = new TextBox
            {
                PlaceholderText = "Enter job description",
                Height = 80,
                AcceptsReturn = true,
                TextWrapping = TextWrapping.Wrap
            };

            txtQualifications = new TextBox
            {
                PlaceholderText = "Enter minimum qualifications",
                Height = 80,
                AcceptsReturn = true,
                TextWrapping = TextWrapping.Wrap
            };

            Button btnPublish = new Button
            {
                Content = "Publish Job Opening",
                Height = 42,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };

            btnPublish.Click += PublishJob_Click;

            StackPanel formPanel = new StackPanel
            {
                Spacing = 12,
                Children =
                {
                    new TextBlock
                    {
                        Text = "Create Job Vacancy",
                        FontSize = 24,
                        FontWeight = FontWeight.Bold,
                        Foreground = Brushes.White
                    },

                    new TextBlock
                    {
                        Text = "Admin/Manager can publish active job openings for applicants.",
                        TextWrapping = TextWrapping.Wrap,
                        Foreground = new SolidColorBrush(Color.Parse("#e0e0e0"))
                    },

                    new TextBlock { Text = "Job Title", Foreground = Brushes.White },
                    txtJobTitle,

                    new TextBlock { Text = "Department", Foreground = Brushes.White },
                    txtDepartment,

                    new TextBlock { Text = "Job Description", Foreground = Brushes.White },
                    txtDescription,

                    new TextBlock { Text = "Minimum Qualifications", Foreground = Brushes.White },
                    txtQualifications,

                    btnPublish
                }
            };

            Border card = new Border
            {
                Background = new SolidColorBrush(Color.Parse("#252525")),
                CornerRadius = new CornerRadius(10),
                Padding = new Thickness(30),
                Width = 450,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Child = formPanel
            };

            Grid mainGrid = new Grid
            {
                Background = new SolidColorBrush(Color.Parse("#1e1e1e")),
                Children =
                {
                    card
                }
            };

            Content = mainGrid;
        }

        private void PublishJob_Click(object? sender, RoutedEventArgs e)
        {
            string jobTitle = txtJobTitle.Text ?? "";
            string department = txtDepartment.Text ?? "";
            string description = txtDescription.Text ?? "";
            string qualifications = txtQualifications.Text ?? "";

            if (jobTitle.Trim() == "" || department.Trim() == "" ||
                description.Trim() == "" || qualifications.Trim() == "")
            {
                ShowMessage("Please fill out all job vacancy fields.");
                return;
            }

            try
            {
                using (MySqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    string insertQuery = $@"INSERT INTO {DatabaseHelper.JobTable}
                               (JobTitle, Department, JobDescription, Qualifications, Status)
                               VALUES
                               (@JobTitle, @Department, @JobDescription, @Qualifications, 'Active')";

                    using (MySqlCommand cmd = new MySqlCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@JobTitle", jobTitle);
                        cmd.Parameters.AddWithValue("@Department", department);
                        cmd.Parameters.AddWithValue("@JobDescription", description);
                        cmd.Parameters.AddWithValue("@Qualifications", qualifications);

                        cmd.ExecuteNonQuery();
                    }

                    ShowMessage("Job vacancy published successfully.");

                    txtJobTitle.Text = "";
                    txtDepartment.Text = "";
                    txtDescription.Text = "";
                    txtQualifications.Text = "";
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Database error: " + ex.Message);
            }
        }

        private async void ShowMessage(string message)
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(async () =>
            {
                Window dialog = new Window
                {
                    Width = 380,
                    Height = 150,
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
            });
        }
    }
}
