using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using HR_Applicant_System.Models;

namespace HR_Applicant_System.ViewModels
{
    public class ApplicantListViewModel : INotifyPropertyChanged
    {
        private readonly ApplicationRepository _repo;
        private ObservableCollection<Application> _applicants;
        private ObservableCollection<Application> _filteredApplicants;
        private Application? _selectedApplicant;
        private string _searchText = string.Empty;
        private string _fullName = string.Empty;
        private string _bio = string.Empty;
        private int _pendingCount;
        private int _vacantCount;
        private int _reviewedCount;

        public ObservableCollection<Application> Applicants
        {
            get => _applicants;
            set
            {
                _applicants = value;
                OnPropertyChanged();
                ApplyFilter();
            }
        }

        public ObservableCollection<Application> FilteredApplicants
        {
            get => _filteredApplicants;
            set
            {
                _filteredApplicants = value;
                OnPropertyChanged();
            }
        }

        public Application? SelectedApplicant
        {
            get => _selectedApplicant;
            set
            {
                _selectedApplicant = value;
                OnPropertyChanged();
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                ApplyFilter();
            }
        }

        public string FullName
        {
            get => _fullName;
            set
            {
                _fullName = value;
                OnPropertyChanged();
            }
        }

        public string Bio
        {
            get => _bio;
            set
            {
                _bio = value;
                OnPropertyChanged();
            }
        }

        public int PendingCount
        {
            get => _pendingCount;
            set
            {
                _pendingCount = value;
                OnPropertyChanged();
            }
        }

        public int VacantCount
        {
            get => _vacantCount;
            set
            {
                _vacantCount = value;
                OnPropertyChanged();
            }
        }

        public int ReviewedCount
        {
            get => _reviewedCount;
            set
            {
                _reviewedCount = value;
                OnPropertyChanged();
            }
        }

        public ApplicantListViewModel()
        {
            _repo = new ApplicationRepository();
            _applicants = new ObservableCollection<Application>();
            _filteredApplicants = new ObservableCollection<Application>();
            RefreshDashboard();
        }

        public ApplicantListViewModel(object userContext) : this()
        {
        }

        public void RefreshDashboard()
        {
            try
            {
                var records = _repo.GetAllActiveApplications();
                if (records != null)
                {
                    var staffReviewOnly = records.FindAll(a => a.Status != "For Final Review");
                    Applicants = new ObservableCollection<Application>(staffReviewOnly);
                    
                    UpdateMetrics(records);
                }
            }
            catch (Exception)
            {
            }
        }

        private void ApplyFilter()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                FilteredApplicants = new ObservableCollection<Application>(Applicants);
                return;
            }

            var filtered = new List<Application>();
            foreach (var app in Applicants)
            {
                if (app.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    app.Position.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                {
                    filtered.Add(app);
                }
            }
            FilteredApplicants = new ObservableCollection<Application>(filtered);
        }

        private void UpdateMetrics(List<Application> allRecords)
        {
            int pending = 0;
            int reviewed = 0;

            foreach (var app in allRecords)
            {
                if (app.Status == "Pending" || app.Status == "Under Review")
                {
                    pending++;
                }
                else if (app.Status == "Hired" || app.Status == "Rejected" || app.Status == "For Final Review")
                {
                    reviewed++;
                }
            }

            PendingCount = pending;
            ReviewedCount = reviewed;
            VacantCount = 5; 
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}