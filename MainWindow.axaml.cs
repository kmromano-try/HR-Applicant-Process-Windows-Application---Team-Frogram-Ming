using Avalonia.Controls;
using Avalonia.Interactivity;
using HR_Applicant_System.Views;
using HR_Applicant_System.ViewModels;

namespace HR_Applicant_System
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Applicant_Click(object? sender, RoutedEventArgs e)
        {
            ApplicantView applicantView = new ApplicantView();
            applicantView.Show();
            this.Close();
        }

        private void Staff_Click(object? sender, RoutedEventArgs e)
        {
            var staffPortalWindow = new Window
            {
                Title = "HR Staff Portal",
                Width = 900,
                Height = 600,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Content = new ApplicantListView
                {
                    DataContext = new ApplicantListViewModel()
                }
            };

            staffPortalWindow.Show();
            this.Close();
        }

        private void Admin_Click(object? sender, RoutedEventArgs e)
        {
            AdminLoginView adminLoginView = new AdminLoginView();
            adminLoginView.Show();
            this.Close();
        }
    }
}