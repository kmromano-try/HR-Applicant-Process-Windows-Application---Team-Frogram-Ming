using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using HR_Applicant_System.ViewModels;
using HR_Applicant_System.Views;

namespace HR_Applicant_System.Views
{
    public partial class ApplicantListView : UserControl
    {
        public ApplicantListView()
        {
            InitializeComponent();

            // Hooking into the "My Profile" button click to open the standalone window
            var btn = this.FindControl<Button>("btnMyProfile");
            if (btn != null) btn.Click += MyProfile_Click;

            // Trigger initial data load when the control is loaded into the visual tree
            this.Loaded += (s, e) =>
            {
                try
                {
                    if (this.DataContext is ApplicantListViewModel vm)
                    {
                        vm.RefreshDashboard.Execute(null);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Initial load error: {ex.Message}");
                }
            };
        }

        public void MyProfile_Click(object? sender, RoutedEventArgs e)
        {
            // Open the standalone profile window and share the current DataContext
            var profileWindow = new StaffProfileView();
            profileWindow.DataContext = this.DataContext;
            profileWindow.Show();
        }

        public void Logout_Click(object? sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            (this.VisualRoot as Window)?.Close();
        }
    }
}