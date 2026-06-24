using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;
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
        private string _currentStatusFilter = "Submitted"; // Default view state
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
        
        // FIX: Sync individual details when selection changes
        if (_selectedApplicant != null)
        {
            FullName = _selectedApplicant.Name;
            Bio = _selectedApplicant.HRRemarks; // Assuming Bio displays current remarks
        }
        else
        {
            FullName = string.Empty;
            Bio = string.Empty;
        }
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
            RefreshDashboardWithFilter(_currentStatusFilter);
        }

        public void RefreshDashboardWithFilter(string statusTarget)
        {
            try
            {
                _currentStatusFilter = statusTarget;
                Console.WriteLine($"[DASHBOARD] Fetching applications filtered by view state: {statusTarget}...");
                var records = _repo.GetAllActiveApplications();
                
                if (records != null)
                {
                    Console.WriteLine($"[DASHBOARD] Successfully read {records.Count} total rows from database.");
                    
                    List<Application> filteredState;

                    if (statusTarget == "Submitted")
                    {
                        // Main Dashboard Pipeline View: Strictly exclude final states (Accepted, Rejected, For Final Review)
                        filteredState = records.Where(a => a.Status == "Submitted" || a.Status == "Under Review").ToList();
                    }
                    else if (statusTarget == "Passed Screening")
                    {
                        // Maps both potential variants safely for the final evaluation queue lists
                        filteredState = records.Where(a => a.Status == "Passed Screening" || a.Status == "For Final Review").ToList();
                    }
                    else
                    {
                        // Fallback matching for explicit states like "Rejected" or "Accepted"
                        filteredState = records.Where(a => a.Status.Equals(statusTarget, StringComparison.OrdinalIgnoreCase)).ToList();
                    }

                    Applicants = new ObservableCollection<Application>(filteredState);
                    UpdateMetrics(records);
                    Console.WriteLine($"[DASHBOARD] Pipeline tracking collection populated with {Applicants.Count} active apps.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[CRITICAL DASHBOARD ERROR] The dashboard loading sequence crashed!");
                Console.WriteLine($"Error Message: {ex.Message}");
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
                if (app.Status == "Pending" || app.Status == "Submitted" || app.Status == "Under Review")
                {
                    pending++;
                }
                else if (app.Status == "Hired" || app.Status == "Accepted" || app.Status == "Rejected" || app.Status == "For Final Review" || app.Status == "Passed Screening")
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