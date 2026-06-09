using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Controls.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;
using HR_Applicant_System.Models;

namespace HR_Applicant_System.Views
{
    public partial class ApplicantView : Window
    {
        private TextBox txtFullName;
        private TextBox txtEmail;
        private TextBox txtContact;
        private ComboBox cbJobs;
        private List<JobVacancy> _availableJobs = new();

        public ApplicantView()
        {
            Width = 500;
            Height = 650;
            Title = "Job Application Portal";
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // Input Fields
            txtFullName = new TextBox { PlaceholderText = "Enter your full name" };
            txtEmail = new TextBox { PlaceholderText = "Enter your email address" };
            txtContact = new TextBox { PlaceholderText = "Enter your contact number" };

            // Dropdown for Jobs
            cbJobs = new ComboBox
            {
                HorizontalAlignment = HorizontalAlignment.Stretch
            };

            // Directly assign hardcoded list of strings for instant UI availability
            cbJobs.ItemsSource = new List<string> { "Junior Python Developer", "Database Administrator", "QA Automation Engineer" };
            cbJobs.SelectedIndex = 0;

            Button btnSubmit = new Button
            {
                Content = "Submit Application",
                Height = 45,
                Background = new SolidColorBrush(Color.Parse("#2563EB")),
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                FontWeight = FontWeight.Bold
            };
            btnSubmit.Click += Submit_Click;

            Button btnBack = new Button
            {
                Content = "Back to Home",
                Height = 35,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            btnBack.Click += (s, e) => { 
                var mainWindow = new HR_Applicant_System.MainWindow();
                mainWindow.Show(); 
                this.Close(); 
            };

            StackPanel formPanel = new StackPanel
            {
                Spacing = 15,
                Children =
                {
                    new TextBlock { Text = "Join Our Team", FontSize = 28, FontWeight = FontWeight.Bold, HorizontalAlignment = HorizontalAlignment.Center },
                    new TextBlock { Text = "Submit your details to apply for an open position.", Foreground = Brushes.Gray, TextWrapping = TextWrapping.Wrap, HorizontalAlignment = HorizontalAlignment.Center },
                    
                    new Separator { Height = 20, Opacity = 0 },

                    new TextBlock { Text = "Full Name", FontWeight = FontWeight.SemiBold },
                    txtFullName,

                    new TextBlock { Text = "Email Address", FontWeight = FontWeight.SemiBold },
                    txtEmail,

                    new TextBlock { Text = "Contact Number", FontWeight = FontWeight.SemiBold },
                    txtContact,

                    new TextBlock { Text = "Desired Position", FontWeight = FontWeight.SemiBold },
                    cbJobs,

                    new Control { Height = 15 },
                    btnSubmit,
                    btnBack
                }
            };

            Border card = new Border
            {
                Background = Brushes.White,
                CornerRadius = new CornerRadius(12),
                Padding = new Thickness(35),
                Width = 420,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Child = formPanel
            };

            Content = new Grid
            {
                Background = new SolidColorBrush(Color.Parse("#F3F4F6")),
                Children = { card }
            };

            LoadJobs();
        }

        private void LoadJobs()
        {
            // Database call bypassed to use hardcoded strings assigned in constructor
        }

        private async void Submit_Click(object? sender, RoutedEventArgs e)
        {
            string name = txtFullName.Text ?? "";
            string email = txtEmail.Text ?? "";
            string contact = txtContact.Text ?? "";
            string selectedJobTitle = cbJobs.SelectedItem as string ?? "";

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) || string.IsNullOrEmpty(selectedJobTitle))
            {
                ShowMessage("Missing Information", "Please provide your Name, Email, and select a Position.");
                return;
            }

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var trans = conn.BeginTransaction())
                    {
                        // 1. Insert or Identify Applicant
                        long applicantId;
                        string checkSql = "INSERT INTO applicants (FullName, Email, ContactNumber) VALUES (@Name, @Email, @Contact) " +
                                          "ON DUPLICATE KEY UPDATE ApplicantID=LAST_INSERT_ID(ApplicantID), ContactNumber=@Contact; SELECT LAST_INSERT_ID();";
                        
                        using (var cmd = new MySqlCommand(checkSql, conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@Name", name);
                            cmd.Parameters.AddWithValue("@Email", email);
                            cmd.Parameters.AddWithValue("@Contact", contact);
                            applicantId = Convert.ToInt64(cmd.ExecuteScalar());
                        }

                        // 2. Create the Application record (JobID 1 used as fallback for hardcoded list)
                        string appSql = "INSERT INTO applications (ApplicantID, JobID, CurrentStatus) VALUES (@Aid, @Jid, 'Submitted')";
                        using (var cmdApp = new MySqlCommand(appSql, conn, trans))
                        {
                            cmdApp.Parameters.AddWithValue("@Aid", applicantId);
                            cmdApp.Parameters.AddWithValue("@Jid", 1);
                            cmdApp.ExecuteNonQuery();
                        }

                        trans.Commit();
                        ShowMessage("Success", "Application submitted! HR will review your profile shortly.");
                        txtFullName.Text = txtEmail.Text = txtContact.Text = "";
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error", $"Could not submit application: {ex.Message}");
            }
        }

        private async void ShowMessage(string title, string message)
        {
            Window dialog = new Window { Width = 380, Height = 160, Title = title, WindowStartupLocation = WindowStartupLocation.CenterOwner, Content = new TextBlock { Text = message, Margin = new Thickness(25), TextWrapping = TextWrapping.Wrap, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center } };
            await dialog.ShowDialog(this);
        }
    }
}