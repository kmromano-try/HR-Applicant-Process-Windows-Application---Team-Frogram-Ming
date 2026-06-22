﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using MySql.Data.MySqlClient;
using HR_Applicant_System.Models;

namespace HR_Applicant_System.Views
{
    public partial class CreateJobView : Window
    {
        public CreateJobView()
        {
            InitializeComponent();
        }

        public void PublishJob_Click(object? sender, RoutedEventArgs e)
        {
            string jobTitle = txtJobTitle?.Text ?? "";
            string department = txtDepartment?.Text ?? "";
            string description = txtDescription?.Text ?? "";
            string qualifications = txtQualifications?.Text ?? "";

            if (string.IsNullOrWhiteSpace(jobTitle) || string.IsNullOrWhiteSpace(department) ||
                string.IsNullOrWhiteSpace(description) || string.IsNullOrWhiteSpace(qualifications))
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
                        cmd.Parameters.AddWithValue("@JobTitle", jobTitle.Trim());
                        cmd.Parameters.AddWithValue("@Department", department.Trim());
                        cmd.Parameters.AddWithValue("@JobDescription", description.Trim());
                        cmd.Parameters.AddWithValue("@Qualifications", qualifications.Trim());
                        cmd.ExecuteNonQuery();
                    }

                    ShowMessage("Job vacancy published successfully.");
                    
                    if (txtJobTitle != null) txtJobTitle.Text = "";
                    if (txtDepartment != null) txtDepartment.Text = "";
                    if (txtDescription != null) txtDescription.Text = "";
                    if (txtQualifications != null) txtQualifications.Text = "";
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Database error: " + ex.Message);
            }
        }

        private void ShowMessage(string message)
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                Window dialog = new Window
                {
                    Width = 380,
                    Height = 150,
                    Title = "Message",
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    Content = new TextBlock
                    {
                        Text = message,
                        Margin = new Thickness(20),
                        TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                        VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
                    }
                };
                dialog.ShowDialog(this);
            });
        }
    }
}