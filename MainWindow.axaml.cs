using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using HR_Applicant_System.Views;
using System;

namespace HR_Applicant_System
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void Applicant_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
                ApplicantView applicantView = new ApplicantView();
                applicantView.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CRITICAL] Error launching Applicant Portal: {ex.Message}");
            }
        }

        public void Staff_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
                StaffLoginView staffLogin = new StaffLoginView();
                staffLogin.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CRITICAL] Error launching Staff Portal: {ex.Message}");
            }
        }

        public void Admin_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
                AdminLoginView adminLoginView = new AdminLoginView();
                adminLoginView.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CRITICAL] Error launching Admin Dashboard: {ex.Message}");
            }
        }
    }
}