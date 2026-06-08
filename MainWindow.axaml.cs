using Avalonia.Controls;
using Avalonia.Interactivity;
using HR_Applicant_System.Views;

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
            StaffView staffView = new StaffView();
            staffView.Show();
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