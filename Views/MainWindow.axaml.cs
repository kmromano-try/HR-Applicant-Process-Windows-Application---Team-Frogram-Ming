using Avalonia.Controls;
using Avalonia.Interactivity;
using HR_Applicant_System.Views;

namespace HR_Applicant_System;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void AdminLogin_Click(object? sender, RoutedEventArgs e)
    {
        AdminView adminView = new AdminView();
        adminView.Show();
        this.Close();
    }
}    
