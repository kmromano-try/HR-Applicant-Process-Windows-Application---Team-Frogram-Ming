using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using HR_Applicant_System.Views;

namespace HR_Applicant_System;

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
            ApplicantView applicantView = new ApplicantView(0);
            applicantView.Show();
            this.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error opening Applicant Portal: {ex.Message}");
        }
    }

    public void Staff_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            var staffView = new HR_Applicant_System.Views.StaffView();
            staffView.Show();
            this.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error opening Staff Portal: {ex.Message}");
        }
    }

    public void Admin_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            // If it requires a login first, change AdminView to AdminLoginView
            var adminView = new HR_Applicant_System.Views.AdminView();
            adminView.Show();
            this.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error opening Admin Portal: {ex.Message}");
        }
    }
}