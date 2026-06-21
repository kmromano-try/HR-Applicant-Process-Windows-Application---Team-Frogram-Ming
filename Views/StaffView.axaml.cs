using Avalonia.Controls;
using Avalonia.Interactivity;

namespace HR_Applicant_System.Views
{
    public partial class StaffView : Window
    {
        public StaffView()
        {
            InitializeComponent();
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
            var mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}