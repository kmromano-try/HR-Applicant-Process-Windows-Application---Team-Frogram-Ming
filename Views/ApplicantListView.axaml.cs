using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using HR_Applicant_System.ViewModels;

namespace HR_Applicant_System.Views
{
    public partial class ApplicantListView : UserControl
    {
        public ApplicantListView()
        {
            InitializeComponent();

            var btn = this.FindControl<Button>("btnMyProfile");
            if (btn != null) btn.Click += MyProfile_Click;

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

        private void MyProfile_Click(object? sender, RoutedEventArgs e)
        {
            var profileWindow = new StaffProfileView();
            profileWindow.DataContext = this.DataContext;
            profileWindow.Show();
        }

        private void Logout_Click(object? sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            (this.VisualRoot as Window)?.Close();
        }
    }
}