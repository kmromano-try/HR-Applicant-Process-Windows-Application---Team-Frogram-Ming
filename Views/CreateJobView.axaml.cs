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
            // Use null-conditional operator (?) to safely access the Text property
            string jobTitle = txtJobTitle?.Text ?? "";
            string dept = txtDepartment?.Text ?? "";
            string desc = txtDescription?.Text ?? "";
            string qual = txtQualifications?.Text ?? "";

            if (string.IsNullOrWhiteSpace(jobTitle) || string.IsNullOrWhiteSpace(dept))
            {
                ShowMessage("Please fill out all mandatory fields.");
                return;
            }

            try
            {
                using (MySqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = $"INSERT INTO {DatabaseHelper.JobTable} (JobTitle, Department, JobDescription, Qualifications, Status) VALUES (@T, @D, @Desc, @Q, 'Active')";
                    
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@T", jobTitle.Trim());
                        cmd.Parameters.AddWithValue("@D", dept.Trim());
                        cmd.Parameters.AddWithValue("@Desc", desc.Trim());
                        cmd.Parameters.AddWithValue("@Q", qual.Trim());
                        cmd.ExecuteNonQuery();
                    }

                    ShowMessage("Job vacancy published successfully.");
                    
                    // Reset fields safely
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
                    Title = "Notification", 
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