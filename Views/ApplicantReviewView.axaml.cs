using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using HR_Applicant_System.Models;
using System;
using System.Data;

namespace HR_Applicant_System.Views
{
    public partial class ApplicantReviewView : Window
    {
        private int _applicationId;
        private int _staffId;

        // Passed in from the Staff Dashboard when they click an applicant
        public ApplicantReviewView(int applicationId, int staffId)
        {
            InitializeComponent();
            _applicationId = applicationId;
            _staffId = staffId;

            LoadApplicantData();
            LockApplicationForReview();
        }

        public ApplicantReviewView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void LoadApplicantData()
        {
            string query = $@"
                SELECT a.FullName, j.JobTitle, a.Bio 
                FROM Applications app
                JOIN Applicants a ON app.ApplicantID = a.ApplicantID
                JOIN Jobs j ON app.JobID = j.JobID
                WHERE app.ApplicationID = {_applicationId}";

            DataTable dt = DatabaseHelper.ExecuteQuery(query);
            if (dt.Rows.Count > 0)
            {
                var fullName = this.FindControl<TextBlock>("FullNameText");
var target = this.FindControl<TextBlock>("TargetPositionText");
var bio = this.FindControl<TextBox>("BioText");

if (fullName != null)
    fullName.Text = "Name: " + dt.Rows[0]["FullName"];

if (target != null)
    target.Text = "Target Position: " + dt.Rows[0]["JobTitle"];

if (bio != null)
    bio.Text = dt.Rows[0]["Bio"].ToString();
            }
        }

        // Checklist Requirement: Lock application for review & Record in History
        private void LockApplicationForReview()
        {
            string updateQuery = $"UPDATE Applications SET Status = 'Under Review' WHERE ApplicationID = {_applicationId}";
            DatabaseHelper.ExecuteNonQuery(updateQuery);

            string historyQuery = $@"
                INSERT INTO ApplicationStatusHistory (ApplicationID, ChangedBy, OldStatus, NewStatus, ChangeDate) 
                VALUES ({_applicationId}, {_staffId}, 'Submitted', 'Under Review', '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')";
            DatabaseHelper.ExecuteNonQuery(historyQuery);
        }

        // Checklist Requirement: View Documents
        private void OnViewDocumentsClicked(object sender, RoutedEventArgs e)
        {
            // In a full implementation, this would open the file path using System.Diagnostics.Process.Start
            // For now, we alert that the document is retrieved.
        }

        // Checklist Requirement: Status Update & Screening Remarks
        private void OnSubmitScreeningClicked(object sender, RoutedEventArgs e)
        {
            var decisionDropdown = this.FindControl<ComboBox>("DecisionDropdown");
            var remarksInput = this.FindControl<TextBox>("RemarksInput");

            if (decisionDropdown == null || remarksInput == null)
                return;

            string newStatus =
                decisionDropdown.SelectedIndex == 0
                    ? "Staff Approved"
                    : "Staff Rejected";
            
            var remarks = remarksInput.Text ?? "";

            // Update final application status
            string updateApp = $"UPDATE Applications SET Status = '{newStatus}', ScreeningRemarks = '{remarks}' WHERE ApplicationID = {_applicationId}";
            DatabaseHelper.ExecuteNonQuery(updateApp);

            // Record the final screening decision into History
            string historyQuery = $@"
                INSERT INTO ApplicationStatusHistory (ApplicationID, ChangedBy, OldStatus, NewStatus, ChangeDate, Remarks) 
                VALUES ({_applicationId}, {_staffId}, 'Under Review', '{newStatus}', '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}', '{remarks}')";
            DatabaseHelper.ExecuteNonQuery(historyQuery);

            this.Close();
        }

        private void OnCancelClicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}   