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
    public class FinalReviewView : Window
    {
        private ListBox applicantList;
        private TextBox txtFinalRemarks;

        public FinalReviewView()
        {
            Width = 750;
            Height = 540;
            Title = "Staff Approved Queue";

            TextBlock title = new TextBlock
            {
                Text = "Staff Approved Queue",
                FontSize = 26,
                FontWeight = FontWeight.Bold
            };

            TextBlock subtitle = new TextBlock
            {
                Text = "Applicants in this section are waiting for Admin/Manager final decision.",
                Foreground = Brushes.Gray,
                TextWrapping = TextWrapping.Wrap
            };

            applicantList = new ListBox
            {
                Height = 220
            };

            txtFinalRemarks = new TextBox
            {
                Watermark = "Enter final remarks here, especially if rejecting applicant...",
                Height = 80,
                AcceptsReturn = true,
                TextWrapping = TextWrapping.Wrap
            };

            Button btnApprove = new Button
            {
                Content = "Final Approve / Hire",
                Height = 42,
                Width = 180
            };

            Button btnReject = new Button
            {
                Content = "Overrule / Reject",
                Height = 42,
                Width = 180
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
                    new TextBlock { Text = "Final Remarks" },
                    txtFinalRemarks,
                    buttonPanel
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

                    string query = @"
                        SELECT 
                            a.ApplicationID,
                            ap.FullName,
                            j.JobTitle,
                            a.CurrentStatus
                        FROM Applications a
                        INNER JOIN Applicants ap ON a.ApplicantID = ap.ApplicantID
                        INNER JOIN JobVacancies j ON a.JobID = j.JobID
                        WHERE a.CurrentStatus = 'For Final Review'";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string item =
                                "Application #" + reader["ApplicationID"] +
                                " | " + reader["FullName"] +
                                " | " + reader["JobTitle"] +
                                " | " + reader["CurrentStatus"];

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

                    string getStatusQuery = "SELECT CurrentStatus, JobID FROM Applications WHERE ApplicationID = @ApplicationID";

                    string oldStatus = "";
                    int jobId = 0;

                    using (MySqlCommand getCmd = new MySqlCommand(getStatusQuery, conn))
                    {
                        getCmd.Parameters.AddWithValue("@ApplicationID", applicationId);

                        using (MySqlDataReader reader = getCmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                oldStatus = reader["CurrentStatus"].ToString() ?? "";
                                jobId = Convert.ToInt32(reader["JobID"]);
                            }
                        }
                    }

                    string updateQuery = "UPDATE Applications SET CurrentStatus = 'Accepted' WHERE ApplicationID = @ApplicationID";

                    using (MySqlCommand updateCmd = new MySqlCommand(updateQuery, conn))
                    {
                        updateCmd.Parameters.AddWithValue("@ApplicationID", applicationId);
                        updateCmd.ExecuteNonQuery();
                    }

                    string closeJobQuery = "UPDATE JobVacancies SET VacancyStatus = 'Closed' WHERE JobID = @JobID";

                    using (MySqlCommand closeCmd = new MySqlCommand(closeJobQuery, conn))
                    {
                        closeCmd.Parameters.AddWithValue("@JobID", jobId);
                        closeCmd.ExecuteNonQuery();
                    }

                    string decisionQuery = @"INSERT INTO HiringDecisions
                                             (ApplicationID, Decision, FinalRemarks)
                                             VALUES
                                             (@ApplicationID, 'Accepted', @FinalRemarks)";

                    using (MySqlCommand decisionCmd = new MySqlCommand(decisionQuery, conn))
                    {
                        decisionCmd.Parameters.AddWithValue("@ApplicationID", applicationId);
                        decisionCmd.Parameters.AddWithValue("@FinalRemarks", txtFinalRemarks.Text ?? "");
                        decisionCmd.ExecuteNonQuery();
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

                    string getStatusQuery = "SELECT CurrentStatus FROM Applications WHERE ApplicationID = @ApplicationID";

                    string oldStatus = "";

                    using (MySqlCommand getCmd = new MySqlCommand(getStatusQuery, conn))
                    {
                        getCmd.Parameters.AddWithValue("@ApplicationID", applicationId);

                        object result = getCmd.ExecuteScalar();
                        oldStatus = result?.ToString() ?? "";
                    }

                    string updateQuery = "UPDATE Applications SET CurrentStatus = 'Rejected' WHERE ApplicationID = @ApplicationID";

                    using (MySqlCommand updateCmd = new MySqlCommand(updateQuery, conn))
                    {
                        updateCmd.Parameters.AddWithValue("@ApplicationID", applicationId);
                        updateCmd.ExecuteNonQuery();
                    }

                    string decisionQuery = @"INSERT INTO HiringDecisions
                                             (ApplicationID, Decision, FinalRemarks)
                                             VALUES
                                             (@ApplicationID, 'Rejected', @FinalRemarks)";

                    using (MySqlCommand decisionCmd = new MySqlCommand(decisionQuery, conn))
                    {
                        decisionCmd.Parameters.AddWithValue("@ApplicationID", applicationId);
                        decisionCmd.Parameters.AddWithValue("@FinalRemarks", remarks);
                        decisionCmd.ExecuteNonQuery();
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

        private async void ShowMessage(string message)
        {
            Window dialog = new Window
            {
                Width = 420,
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
