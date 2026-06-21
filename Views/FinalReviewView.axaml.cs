﻿using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using MySql.Data.MySqlClient;
using HR_Applicant_System.Models;

namespace HR_Applicant_System.Views
{
    public class FinalReviewView : Window
    {
        private ListBox applicantList;
        private TextBox txtFinalRemarks;

        public FinalReviewView()
        {
            Width = 750;
            Height = 540;
            Title = "Staff Approved Queue";
            WindowStartupLocation = WindowStartupLocation.CenterOwner;

            TextBlock title = new TextBlock
            {
                Text = "Staff Approved Queue",
                FontSize = 26,
                FontWeight = FontWeight.Bold,
                Foreground = Brushes.White
            };

            TextBlock subtitle = new TextBlock
            {
                Text = "Applicants in this section are waiting for Admin/Manager final decision.",
                Foreground = new SolidColorBrush(Color.Parse("#e0e0e0")),
                TextWrapping = TextWrapping.Wrap
            };

            applicantList = new ListBox
            {
                Height = 220,
                Background = new SolidColorBrush(Color.Parse("#1e1e1e")),
                Foreground = Brushes.White
            };
            
            applicantList.DoubleTapped += ApplicantList_DoubleTapped;

            txtFinalRemarks = new TextBox
            {
                PlaceholderText = "Enter final remarks here, especially if rejecting applicant...",
                Height = 80,
                AcceptsReturn = true,
                TextWrapping = TextWrapping.Wrap,
                Background = new SolidColorBrush(Color.Parse("#1e1e1e")),
                Foreground = Brushes.White
            };

            Button btnApprove = new Button
            {
                Content = "Final Approve / Hire",
                Height = 42,
                Width = 180,
                Background = new SolidColorBrush(Color.Parse("#10B981")),
                Foreground = Brushes.White,
                FontWeight = FontWeight.Bold,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center
            };

            Button btnReject = new Button
            {
                Content = "Overrule / Reject",
                Height = 42,
                Width = 180,
                Background = new SolidColorBrush(Color.Parse("#EF4444")),
                Foreground = Brushes.White,
                FontWeight = FontWeight.Bold,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center
            };

            btnApprove.Click += Approve_Click;
            btnReject.Click += Reject_Click;

            StackPanel buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Spacing = 15,
                Children =
                {
                    btnApprove,
                    btnReject
                }
            };

            StackPanel contentPanel = new StackPanel
            {
                Spacing = 15,
                Children =
                {
                    title,
                    subtitle,
                    applicantList,
                    new TextBlock { Text = "Final Remarks", Foreground = Brushes.White, FontWeight = FontWeight.SemiBold },
                    txtFinalRemarks,
                    buttonPanel
                }
            };

            Border card = new Border
            {
                Background = new SolidColorBrush(Color.Parse("#252525")),
                CornerRadius = new CornerRadius(10),
                Padding = new Thickness(30),
                Margin = new Thickness(25),
                Child = contentPanel
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

            LoadFinalReviewApplicants();
        }

        private void LoadFinalReviewApplicants()
        {
            applicantList.Items.Clear();

            try
            {
                using (MySqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    string query = $@"
                        SELECT 
                            a.ApplicationID,
                            ap.FullName,
                            j.JobTitle,
                            a.Status
                        FROM {DatabaseHelper.ApplicationTable} a
                        INNER JOIN {DatabaseHelper.ApplicantTable} ap ON a.ApplicantID = ap.ApplicantID
                        INNER JOIN {DatabaseHelper.JobTable} j ON a.VacancyID = j.VacancyID
                        WHERE a.Status = 'For Final Review'";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string item =
                                "Application #" + reader["ApplicationID"] +
                                " | " + reader["FullName"] +
                                " | " + reader["JobTitle"] +
                                " | " + reader["Status"];

                            applicantList.Items.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Database error: " + ex.Message);
            }
        }

        public void ApplicantList_DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
        {
            int applicationId = GetSelectedApplicationID();
            if (applicationId > 0)
            {
                var detailWindow = new ApplicantDetailView(applicationId);
                detailWindow.ShowDialog(this);
            }
        }

        private int GetSelectedApplicationID()
        {
            if (applicantList.SelectedItem == null)
            {
                ShowMessage("Please select an applicant first.");
                return 0;
            }

            string selected = applicantList.SelectedItem.ToString() ?? "";
            string firstPart = selected.Split('|')[0].Trim();
            string idText = firstPart.Replace("Application #", "").Trim();

            return Convert.ToInt32(idText);
        }

        private void Approve_Click(object? sender, RoutedEventArgs e)
        {
            int applicationId = GetSelectedApplicationID();

            if (applicationId == 0)
                return;

            try
            {
                using (MySqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    string getStatusQuery = $"SELECT Status, VacancyID FROM {DatabaseHelper.ApplicationTable} WHERE ApplicationID = @ApplicationID";

                    string oldStatus = "";
                    int vacancyId = 0;

                    using (MySqlCommand getCmd = new MySqlCommand(getStatusQuery, conn))
                    {
                        getCmd.Parameters.AddWithValue("@ApplicationID", applicationId);

                        using (MySqlDataReader reader = getCmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                oldStatus = reader["Status"].ToString() ?? "";
                                vacancyId = Convert.ToInt32(reader["VacancyID"]);
                            }
                        }
                    }

                    using (MySqlCommand updateCmd = new MySqlCommand($"UPDATE {DatabaseHelper.ApplicationTable} SET Status = 'Accepted' WHERE ApplicationID = @ApplicationID", conn))
                    {
                        updateCmd.Parameters.AddWithValue("@ApplicationID", applicationId);
                        updateCmd.ExecuteNonQuery();
                    }

                    string closeJobQuery = $"UPDATE {DatabaseHelper.JobTable} SET Status = 'Closed' WHERE VacancyID = @VacancyID";

                    using (MySqlCommand closeCmd = new MySqlCommand(closeJobQuery, conn))
                    {
                        closeCmd.Parameters.AddWithValue("@VacancyID", vacancyId);
                        closeCmd.ExecuteNonQuery();
                    }

                    string historyQuery = @"INSERT INTO ApplicationStatusHistory
                                            (ApplicationID, OldStatus, NewStatus, Remarks)
                                            VALUES
                                            (@ApplicationID, @OldStatus, 'Accepted', @Remarks)";

                    using (MySqlCommand historyCmd = new MySqlCommand(historyQuery, conn))
                    {
                        historyCmd.Parameters.AddWithValue("@ApplicationID", applicationId);
                        historyCmd.Parameters.AddWithValue("@OldStatus", oldStatus);
                        historyCmd.Parameters.AddWithValue("@Remarks", txtFinalRemarks.Text ?? "");
                        historyCmd.ExecuteNonQuery();
                    }

                    ShowMessage("Applicant has been accepted successfully.");
                    txtFinalRemarks.Text = "";
                    LoadFinalReviewApplicants();
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Database error: " + ex.Message);
            }
        }

        private void Reject_Click(object? sender, RoutedEventArgs e)
        {
            int applicationId = GetSelectedApplicationID();

            if (applicationId == 0)
                return;

            string remarks = txtFinalRemarks.Text ?? "";

            if (remarks.Trim() == "")
            {
                ShowMessage("Please enter final remarks before rejecting the applicant.");
                return;
            }

            try
            {
                using (MySqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    string getStatusQuery = $"SELECT Status FROM {DatabaseHelper.ApplicationTable} WHERE ApplicationID = @ApplicationID";

                    string oldStatus = "";

                    using (MySqlCommand getCmd = new MySqlCommand(getStatusQuery, conn))
                    {
                        getCmd.Parameters.AddWithValue("@ApplicationID", applicationId);

                        object result = getCmd.ExecuteScalar();
                        oldStatus = result?.ToString() ?? "";
                    }

                    string updateQuery = $"UPDATE {DatabaseHelper.ApplicationTable} SET Status = 'Rejected' WHERE ApplicationID = @ApplicationID";

                    using (MySqlCommand updateCmd = new MySqlCommand(updateQuery, conn))
                    {
                        updateCmd.Parameters.AddWithValue("@ApplicationID", applicationId);
                        updateCmd.ExecuteNonQuery();
                    }

                    string historyQuery = @"INSERT INTO ApplicationStatusHistory
                                            (ApplicationID, OldStatus, NewStatus, Remarks)
                                            VALUES
                                            (@ApplicationID, @OldStatus, 'Rejected', @Remarks)";

                    using (MySqlCommand historyCmd = new MySqlCommand(historyQuery, conn))
                    {
                        historyCmd.Parameters.AddWithValue("@ApplicationID", applicationId);
                        historyCmd.Parameters.AddWithValue("@OldStatus", oldStatus);
                        historyCmd.Parameters.AddWithValue("@Remarks", remarks);
                        historyCmd.ExecuteNonQuery();
                    }

                    ShowMessage("Applicant has been rejected successfully.");
                    txtFinalRemarks.Text = "";
                    LoadFinalReviewApplicants();
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Database error: " + ex.Message);
            }
        }

        private void ShowMessage(string message)
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(async () =>
            {
                Window dialog = new Window
                {
                    Width = 420,
                    Height = 160,
                    Title = "System Notification",
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    Background = new SolidColorBrush(Color.Parse("#252525")),
                    Content = new TextBlock
                    {
                        Text = message,
                        Margin = new Thickness(20),
                        Foreground = Brushes.White,
                        TextWrapping = TextWrapping.Wrap,
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        FontSize = 14
                    }
                };

                await dialog.ShowDialog(this);
            });
        }
    }
}