using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ReactiveUI;

namespace HR_Applicant_System.ViewModels
{
    public class ApplicantListViewModel : ViewModelBase
    {
        private string _staffEmail = "hugh.franco@company.edu.ph";
        public string StaffEmail
        {
            get => _staffEmail;
            set => this.RaiseAndSetIfChanged(ref _staffEmail, value);
        }

        private string _fullName = "Hugh Gabriel Franco";
        public string FullName
        {
            get => _fullName;
            set => this.RaiseAndSetIfChanged(ref _fullName, value);
        }

        private DateTimeOffset? _birthdate = new DateTimeOffset(2005, 1, 1, 0, 0, 0, TimeSpan.Zero);
        public DateTimeOffset? Birthdate
        {
            get => _birthdate;
            set => this.RaiseAndSetIfChanged(ref _birthdate, value);
        }

        private string _department = "Human Resources Development";
        public string Department
        {
            get => _department;
            set => this.RaiseAndSetIfChanged(ref _department, value);
        }

        private string _bio = "Lead HR Specialist coordinating the Capstone Evaluation Systems pipeline.";
        public string Bio
        {
            get => _bio;
            set => this.RaiseAndSetIfChanged(ref _bio, value);
        }

        public ObservableCollection<ApplicantItem> Applicants { get; set; }

        public ICommand SaveProfile { get; }
        public ICommand RefreshDashboard { get; }

        public ApplicantListViewModel()
        {
            Applicants = new ObservableCollection<ApplicantItem>();
            
            LoadDataFromDatabase();

            SaveProfile = ReactiveCommand.Create(() =>
            {
                System.Diagnostics.Debug.WriteLine($"[HR Portal] Profile saved successfully for {FullName}.");
            });

            RefreshDashboard = ReactiveCommand.Create(() =>
            {
                LoadDataFromDatabase();
            });
        }

        private void LoadDataFromDatabase()
        {
            try
            {
                var repo = new ApplicationRepository();
                var databaseRecords = repo.GetAllActiveApplications();

                Applicants.Clear();
                foreach (var item in databaseRecords)
                {
                    Applicants.Add(item);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Database connection failed: {ex.Message}");
                
                Applicants.Clear();
                Applicants.Add(new ApplicantItem { Name = "Alice Juan (Mock Data)", Position = "Junior Python Developer", Status = "Pending Review" });
                Applicants.Add(new ApplicantItem { Name = "Mark Carandang (Mock Data)", Position = "Database Administrator", Status = "Interviewing" });
                Applicants.Add(new ApplicantItem { Name = "Sophia Mendoza (Mock Data)", Position = "QA Automation Engineer", Status = "Technical Test" });
            }
        }
    }

    public class ApplicantItem
    {
        public string Name { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}   