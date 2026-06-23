using System;
using System.Data;
using Avalonia;
using Avalonia.Controls;
using HR_Applicant_System.Models; // Required to connect to your DatabaseHelper

namespace HR_Applicant_System.Views
{
    public partial class StaffDashboardView : Window
    {
        private ListBox _newAppsListBox = new ListBox();
        private ListBox _reviewedAppsListBox = new ListBox();

        public StaffDashboardView()
        {
            InitializeComponent();
            SetupDashboard();
            LoadData();
        }

        private void SetupDashboard()
        {
            // Ensure your StaffDashboardView.axaml has <TabControl Name="DashboardTabs">
            var tabControl = this.FindControl<TabControl>("DashboardTabs");

            if (tabControl == null) return;

            // Tab 1: New Applications
            TabItem newAppsTab = new TabItem { Header = "New Applications" };
            newAppsTab.Content = _newAppsListBox;
            _newAppsListBox.SelectionChanged += OnNewApplicationSelected;

            // Tab 2: Reviewed Applications
            TabItem reviewedAppsTab = new TabItem { Header = "Reviewed Applications" };
            reviewedAppsTab.Content = _reviewedAppsListBox;

            tabControl.Items.Add(newAppsTab);
            tabControl.Items.Add(reviewedAppsTab);
        }

        private void OnNewApplicationSelected(object? sender, SelectionChangedEventArgs e)
        {
            // Safely get the selected item
            var selectedApp = _newAppsListBox.SelectedItem as DataRowView;
            if (selectedApp == null) return;

            // Get the ID (Ensure "ApplicationID" matches your database column name)
            string appId = selectedApp["ApplicationID"].ToString() ?? "";

            // 1. Update status to 'Under Review'
            string updateQuery = $"UPDATE Applications SET Status = 'Under Review' WHERE ApplicationID = '{appId}'";
            DatabaseHelper.ExecuteNonQuery(updateQuery);

            // 2. Add history record
            string historyQuery = $"INSERT INTO ApplicationStatusHistory (ApplicationID, Status, DateChanged) VALUES ('{appId}', 'Under Review', NOW())";
            DatabaseHelper.ExecuteNonQuery(historyQuery);

            // 3. Refresh lists
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                // Populate New Applications
                string queryNew = "SELECT * FROM Applications WHERE Status = 'Submitted'";
                _newAppsListBox.ItemsSource = DatabaseHelper.ExecuteQuery(queryNew).DefaultView;

                // Populate Reviewed Applications
                string queryReviewed = "SELECT * FROM Applications WHERE Status IN ('Under Review', 'Shortlisted', 'For Interview', 'For Assessment')";
                _reviewedAppsListBox.ItemsSource = DatabaseHelper.ExecuteQuery(queryReviewed).DefaultView;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Database error in LoadData: " + ex.Message);
            }
        }

        private void InitializeComponent()
        {
            Avalonia.Markup.Xaml.AvaloniaXamlLoader.Load(this);
        }
    }
}