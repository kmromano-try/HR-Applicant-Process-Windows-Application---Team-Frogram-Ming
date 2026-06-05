using Avalonia.Controls;
using Avalonia.Interactivity;

namespace HR_Applicant_System.Views
{
    public partial class AdminView : Window
    {
        public AdminView()
        {
            InitializeComponent();
        }

        private void CreateStaff_Click(object? sender, RoutedEventArgs e)
        {
            CreateStaffView createStaffView = new CreateStaffView();
            createStaffView.Show();
        }
        private void CreateJob_Click(object? sender, RoutedEventArgs e)
        {
            CreateJobView createJobView = new CreateJobView();
            createJobView.Show();
        }
        private void FinalReview_Click(object? sender, RoutedEventArgs e)
        {
            FinalReviewView finalReviewView = new FinalReviewView();
            finalReviewView.Show();
        }
        private void RejectedQueue_Click(object? sender, RoutedEventArgs e)
        {
            RejectedQueueView rejectedQueueView = new RejectedQueueView();
            rejectedQueueView.Show();
        }
        private void Logout_Click(object? sender, RoutedEventArgs e)
        {
            HR_Applicant_System.MainWindow mainWindow = new HR_Applicant_System.MainWindow();
            mainWindow.Show();
            this.Close();
        }
        private void Reports_Click(object? sender, RoutedEventArgs e)
        {
            ReportsView reportsView = new ReportsView();
            reportsView.Show();
        }
    }
}