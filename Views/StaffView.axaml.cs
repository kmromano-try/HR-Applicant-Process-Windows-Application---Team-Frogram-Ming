using Avalonia.Controls;
using Avalonia.Interactivity;
using System;

namespace HR_Applicant_System.Views
{
    public partial class StaffView : Window
    {
        public StaffView()
        {
            InitializeComponent();
        }

        private void Logout_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
                // Safely drop back to the main portal menu window upon logging out
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CRITICAL] Error handling staff logout sequence: {ex.Message}");
            }
        }
    }
}