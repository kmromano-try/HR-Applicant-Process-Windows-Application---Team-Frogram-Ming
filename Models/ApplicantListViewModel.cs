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

            var repo = new ApplicationRepository();
            bool success = await Task.Run(() => repo.UpdateApplicationStatus(SelectedApplicant.ApplicationID, "For Final Review"));

            if (success)
            {
                await ExecuteRefreshDashboard();
            }
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
                Dispatcher.UIThread.Post(() => UpdateApplicantsList(records));
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