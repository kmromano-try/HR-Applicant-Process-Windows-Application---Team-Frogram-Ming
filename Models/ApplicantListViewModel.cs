using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Windows.Input;
using ReactiveUI;
using HR_Applicant_System.Models;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using System.Threading.Tasks;

namespace HR_Applicant_System.ViewModels
{
    public class AsyncCommand : ICommand
    {
        private readonly Func<Task> _execute;
        private bool _isExecuting;

        public AsyncCommand(Func<Task> execute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        }

        public bool CanExecute(object? parameter)
        {
            return !_isExecuting;
        }

        public async void Execute(object? parameter)
        {
            if (_isExecuting) return;

            try
            {
                _isExecuting = true;
                RaiseCanExecuteChanged();
                await _execute();
            }
            finally
            {
                _isExecuting = false;
                RaiseCanExecuteChanged();
            }
        }

        public event EventHandler? CanExecuteChanged;

        private void RaiseCanExecuteChanged()
        {
            Dispatcher.UIThread.Post(() =>
            {
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            });
        }
    }

    public class ApplicantListViewModel : ReactiveObject
    {
        private string _staffEmail = string.Empty;
        public string StaffEmail
        {
            get => _staffEmail;
            set => this.RaiseAndSetIfChanged(ref _staffEmail, value);
        }

        private string _fullName = string.Empty;
        public string FullName
        {
            get => _fullName;
            set => this.RaiseAndSetIfChanged(ref _fullName, value);
        }

        private DateTimeOffset? _birthdate = null;
        public DateTimeOffset? Birthdate
        {
            get => _birthdate;
            set => this.RaiseAndSetIfChanged(ref _birthdate, value);
        }

        private string _department = string.Empty;
        public string Department
        {
            get => _department;
            set => this.RaiseAndSetIfChanged(ref _department, value);
        }

        private string _bio = string.Empty;
        public string Bio
        {
            get => _bio;
            set => this.RaiseAndSetIfChanged(ref _bio, value);
        }

        private ApplicantItem? _selectedApplicant;
        public ApplicantItem? SelectedApplicant
        {
            get => _selectedApplicant;
            set => this.RaiseAndSetIfChanged(ref _selectedApplicant, value);
        }

        public ObservableCollection<ApplicantItem> Applicants { get; set; }

        public ICommand SaveProfile { get; }
        public ICommand RefreshDashboard { get; }
        public ICommand PassApplication { get; }
        public ICommand FailApplication { get; }

        public ApplicantListViewModel(string email = "hugh.franco@company.edu.ph")
        {
            Applicants = new ObservableCollection<ApplicantItem>();
            StaffEmail = email;
            FullName = string.Empty;
            Bio = string.Empty;
            Department = email == "hugh.franco@company.edu.ph" ? "Human Resources Development" : "";

            SaveProfile = new AsyncCommand(ExecuteSaveProfile);
            RefreshDashboard = new AsyncCommand(ExecuteRefreshDashboard);
            PassApplication = new AsyncCommand(ExecutePassApplication);
            FailApplication = new AsyncCommand(ExecuteFailApplication);
        }

        private async Task ExecuteSaveProfile()
        {
            await Task.Run(() =>
            {
                Dispatcher.UIThread.Post(() =>
                {
                    var dialog = new Window
                    {
                        Width = 380, Height = 160, Title = "Save Success",
                        WindowStartupLocation = WindowStartupLocation.CenterScreen,
                        Background = Brush.Parse("#1e1e1e"),
                        Content = new TextBlock { Text = "Profile changes saved successfully!", Foreground = Brushes.White, Margin = new Thickness(25), TextWrapping = TextWrapping.Wrap, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center }
                    };
                    dialog.Show();
                });
            });
        }

        private async Task ExecutePassApplication()
        {
            if (SelectedApplicant == null) return;

            int applicantId = SelectedApplicant.ApplicationID;
            string applicantName = SelectedApplicant.Name;

            Dispatcher.UIThread.Post(() =>
            {
                var interviewWindow = new Window
                {
                    Title = $"Schedule Interview - {applicantName}",
                    Width = 400,
                    Height = 440,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    Background = Brush.Parse("#1e1e1e")
                };

                var mainStack = new StackPanel { Margin = new Thickness(20), Spacing = 10 };

                mainStack.Children.Add(new TextBlock { Text = "File Interview Information", FontSize = 18, FontWeight = FontWeight.Bold, Foreground = Brushes.White, Margin = new Thickness(0, 0, 0, 10) });

                mainStack.Children.Add(new TextBlock { Text = "Interviewer Name:", Foreground = Brushes.LightGray });
                var txtInterviewer = new TextBox { PlaceholderText = "e.g., HR Manager / Staff Name", Height = 35 };
                mainStack.Children.Add(txtInterviewer);

                mainStack.Children.Add(new TextBlock { Text = "Interview Date (YYYY-MM-DD):", Foreground = Brushes.LightGray });
                var txtDate = new TextBox { PlaceholderText = "e.g., 2026-06-25", Height = 35 };
                mainStack.Children.Add(txtDate);

                mainStack.Children.Add(new TextBlock { Text = "Interview Time (e.g., 10:00 AM):", Foreground = Brushes.LightGray });
                var txtTime = new TextBox { PlaceholderText = "e.g., 2:30 PM", Height = 35 };
                mainStack.Children.Add(txtTime);

                mainStack.Children.Add(new TextBlock { Text = "Initial Staff Evaluation Remarks:", Foreground = Brushes.LightGray });
                var txtRemarks = new TextBox { PlaceholderText = "Enter evaluation notes or initial requirements status...", Height = 70, AcceptsReturn = true };
                mainStack.Children.Add(txtRemarks);

                var btnStack = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 10, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(0, 15, 0, 0) };
                
                var btnCancel = new Button { Content = "Cancel", Padding = new Thickness(15, 7) };
                btnCancel.Click += (s, ev) => interviewWindow.Close();

                var btnConfirm = new Button { Content = "Confirm & Send to Admin", Background = Brush.Parse("#10B981"), Foreground = Brushes.White, Padding = new Thickness(15, 7), FontWeight = FontWeight.Bold };
                btnConfirm.Click += async (s, ev) =>
                {
                    if (string.IsNullOrWhiteSpace(txtDate.Text) || string.IsNullOrWhiteSpace(txtTime.Text))
                    {
                        return;
                    }

                    await Task.Run(() =>
                    {
                        var repo = new ApplicationRepository();
                        repo.UpdateApplicationStatus(applicantId, "For Final Review");
                    });

                    interviewWindow.Close();
                    await ExecuteRefreshDashboard();
                };

                btnStack.Children.Add(btnCancel);
                btnStack.Children.Add(btnConfirm);
                mainStack.Children.Add(btnStack);

                interviewWindow.Content = mainStack;
                interviewWindow.Show();
            });
        }

        private async Task ExecuteFailApplication()
        {
            if (SelectedApplicant == null) return;

            var repo = new ApplicationRepository();
            bool success = await Task.Run(() => repo.UpdateApplicationStatus(SelectedApplicant.ApplicationID, "Rejected"));

            if (success)
            {
                await ExecuteRefreshDashboard();
            }
        }

        public async Task ExecuteRefreshDashboard()
        {
            try
            {
                var repo = new ApplicationRepository();
                var records = await Task.Run(() => repo.GetAllActiveApplications() ?? new List<ApplicantItem>());
                var staffReviewOnly = records.FindAll(a => a.Status != "For Final Review" && a.Status != "Rejected");

                Dispatcher.UIThread.Post(() => UpdateApplicantsList(staffReviewOnly));
            }
            catch (Exception ex)
            {
                HandleSyncError(ex);
            }
        }

        private void UpdateApplicantsList(List<ApplicantItem> databaseRecords)
        {
            try
            {
                Applicants.Clear();
                if (databaseRecords != null)
                {
                    foreach (var item in databaseRecords) Applicants.Add(item);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Collection Update Error: {ex.Message}");
            }
        }

        private void HandleSyncError(Exception ex)
        {
            Dispatcher.UIThread.Post(() =>
            {
                var dialog = new Window
                {
                    Width = 400, Height = 180, Title = "Sync Failed",
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    Content = new TextBlock { Text = "Failed to synchronize workspace data with the server.\n\nDetails: " + ex.Message, Margin = new Thickness(25), TextWrapping = TextWrapping.Wrap, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, Foreground = Brushes.White },
                    Background = Brush.Parse("#1e1e1e")
                };
                dialog.Show();

                Applicants.Clear();
                Applicants.Add(new ApplicantItem { Name = "Alice Juan (Mock Data)", Position = "Junior Python Developer", Status = "Pending Review" });
                Applicants.Add(new ApplicantItem { Name = "Mark Carandang (Mock Data)", Position = "Database Administrator", Status = "Interviewing" });
                Applicants.Add(new ApplicantItem { Name = "Sophia Mendoza (Mock Data)", Position = "QA Automation Engineer", Status = "Technical Test" });
            });
        }
    }

    public class ApplicantItem
    {
        public int ApplicationID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}