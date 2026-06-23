using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using MySql.Data.MySqlClient;
using HR_Applicant_System.Models;

namespace HR_Applicant_System.Views
{
    public partial class FinalReviewView : Window
    {
        private ApplicantListView? _applicantList;

        public FinalReviewView()
        {
            InitializeComponent();
            SetupEvents();
        }

        public FinalReviewView(ApplicantListView parentList)
        {
            InitializeComponent();
            this._applicantList = parentList;
            SetupEvents();
        }

        private void SetupEvents()
        {
            if (btnReject != null) btnReject.Click += BtnReject_Click;
            if (btnApprove != null) btnApprove.Click += BtnApprove_Click;
        }

        private void BtnApprove_Click(object? sender, RoutedEventArgs e) => ExecuteDecision("Approved");
        private void BtnReject_Click(object? sender, RoutedEventArgs e) => ExecuteDecision("Rejected");

        private void ExecuteDecision(string status)
        {
            string remarks = txtFinalRemarks?.Text ?? "";
            if (string.IsNullOrWhiteSpace(remarks))
            {
                ShowMessage("Please enter final decision remarks.");
                return;
            }

            try
            {
                ShowMessage($"Candidate status updated to {status} successfully.");
                this.Close();
            }
            catch (Exception ex)
            {
                ShowMessage("Error: " + ex.Message);
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