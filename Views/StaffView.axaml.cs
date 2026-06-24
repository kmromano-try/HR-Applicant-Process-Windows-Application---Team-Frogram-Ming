using Avalonia.Controls;
using Avalonia.Interactivity;
using HR_Applicant_System.ViewModels;
using System;

namespace HR_Applicant_System.Views
{
    public partial class StaffView : Window
    {
        private ApplicantListViewModel _viewModel;

        public StaffView()
{
    InitializeComponent();
    
    _viewModel = new ApplicantListViewModel();
    this.DataContext = _viewModel;
    
    // FIX: Force the child list control to explicitly share the window's view model instance
    MainApplicantList.DataContext = _viewModel;
    
    LoadPipelineData("Submitted");
}

        private void MainPipeline_Click(object? sender, RoutedEventArgs e)
        {
            LoadPipelineData("Submitted");
        }

        private void FinalReview_Click(object? sender, RoutedEventArgs e)
        {
            // Changes view to display only records passed by staff waiting for admin action
            LoadPipelineData("Passed Screening"); 
        }

        private void RejectedQueue_Click(object? sender, RoutedEventArgs e)
        {
            LoadPipelineData("Rejected");
        }

        private void LoadPipelineData(string statusTarget)
        {
            if (_viewModel != null)
            {
                // We'll wire this custom method up in the ViewModel next to filter out the lists
                _viewModel.RefreshDashboardWithFilter(statusTarget);
            }
        }

        private void Logout_Click(object? sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}