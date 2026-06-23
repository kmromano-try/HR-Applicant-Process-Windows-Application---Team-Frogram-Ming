using Avalonia.Controls;
using Avalonia.Interactivity;
using HR_Applicant_System.ViewModels;
using System;

namespace HR_Applicant_System.Views
{
    public partial class StaffView : Window
    {
        public StaffView()
        {
            InitializeComponent();
            
            // Centralize data routing at the parent window level
            var viewModel = new ApplicantListViewModel();
            this.DataContext = viewModel;
            viewModel.RefreshDashboard();
        }

        private void FinalReview_Click(object? sender, RoutedEventArgs e)
        {
            var finalReviewWindow = new FinalReviewView();
            finalReviewWindow.ShowDialog(this);
        }

        private void RejectedQueue_Click(object? sender, RoutedEventArgs e)
        {
            var rejectedQueueWindow = new RejectedQueueView();
            rejectedQueueWindow.ShowDialog(this);
        }

        private void Logout_Click(object? sender, RoutedEventArgs e)
        {
            var StaffLoginView = new StaffLoginView();
            StaffLoginView.Show();
            this.Close();
        }
    }
}