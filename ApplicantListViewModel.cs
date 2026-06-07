using System;
using System.Collections.ObjectModel;

namespace HR_Applicant_System.ViewModels
{
    public class ApplicantListViewModel : ViewModelBase
    {
        private readonly ApplicationRepository _repository = new ApplicationRepository();

        public ObservableCollection<JobVacancySummary> PendingJobs { get; set; } = new ObservableCollection<JobVacancySummary>();
        public ObservableCollection<ApplicantDetailsSummary> JobApplicants { get; set; } = new ObservableCollection<ApplicantDetailsSummary>();
        public ObservableCollection<JobVacancySummary> EmptyJobs { get; set; } = new ObservableCollection<JobVacancySummary>();
        public ObservableCollection<ReviewedApplicationSummary> ReviewedApplications { get; set; } = new ObservableCollection<ReviewedApplicationSummary>();

        private JobVacancySummary? _selectedJob;
        public JobVacancySummary? SelectedJob
        {
            get => _selectedJob;
            set
            {
                _selectedJob = value;
                OnPropertyChanged(nameof(SelectedJob));
                LoadApplicantsForSelectedJob();
            }
        }

        private ApplicantDetailsSummary? _selectedApplicant;
        public ApplicantDetailsSummary? SelectedApplicant
        {
            get => _selectedApplicant;
            set
            {
                _selectedApplicant = value;
                OnPropertyChanged(nameof(SelectedApplicant));
            }
        }

        public string StaffEmail { get; set; } = "staff@company.com";
        public string FullName { get; set; } = "";
        public DateTimeOffset Birthdate { get; set; } = DateTimeOffset.Now;
        public string Bio { get; set; } = "";
        public string Department { get; set; } = "";

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(nameof(IsLoading)); }
        }

        public ApplicantListViewModel()
        {
            RefreshDashboard();
        }

        public void RefreshDashboard()
        {
            IsLoading = true;
            PendingJobs.Clear();
            EmptyJobs.Clear();
            ReviewedApplications.Clear();

            foreach (var j in _repository.GetJobsWithPendingApplications()) PendingJobs.Add(j);
            foreach (var j in _repository.GetJobsWithoutApplications()) EmptyJobs.Add(j);
            foreach (var r in _repository.GetReviewedApplications()) ReviewedApplications.Add(r);

            IsLoading = false;
        }

        private void LoadApplicantsForSelectedJob()
        {
            JobApplicants.Clear();
            SelectedApplicant = null;
            if (SelectedJob != null)
            {
                foreach (var app in _repository.GetApplicantsForJob(SelectedJob.JobVacancyID))
                {
                    JobApplicants.Add(app);
                }
            }
        }

        public void AcceptCurrentApplication() => ProcessDecision("Staff Approved");
        public void RejectCurrentApplication() => ProcessDecision("Staff Rejected");

        private void ProcessDecision(string decision)
        {
            if (SelectedApplicant != null)
            {
                bool success = _repository.ReviewApplication(SelectedApplicant.ApplicationID, decision, "Processed by HR Staff", "HR Staff Member");
                if (success)
                {
                    RefreshDashboard();
                    JobApplicants.Clear();
                    SelectedApplicant = null;
                }
            }
        }

        public void SaveProfile()
        {
            System.Diagnostics.Debug.WriteLine($"Profile saved for {FullName}");
        }
    }
}