using Avalonia.Controls;
using System;
using System.Threading.Tasks;
using HR_Applicant_System.Models;

namespace HR_Applicant_System.Views
{
    public partial class FinalReviewView : Window
    {
        private readonly ApplicationRepository _repo;

        public FinalReviewView()
        {
            InitializeComponent();
            _repo = new ApplicationRepository();

            btnApprove.Click += async (sender, e) => await ProcessDecision("Hired");
            btnReject.Click += async (sender, e) => await ProcessDecision("Rejected");
        }

        private async Task ProcessDecision(string targetStatus)
        {
            var selectedApplicant = applicantList.SelectedItem as Application;
            if (selectedApplicant == null)
            {
                return;
            }

            selectedApplicant.Status = targetStatus;
            selectedApplicant.HRRemarks = txtFinalRemarks.Text ?? string.Empty;

            try
            {
                await Task.Run(() => _repo.UpdateApplicationStatus(selectedApplicant));
                txtFinalRemarks.Text = string.Empty;
                await RefreshList();
            }
            catch (Exception)
            {
            }
        }

        private async Task RefreshList()
        {
            try
            {
                var records = await Task.Run(() => _repo.GetAllActiveApplications());
                if (records != null)
                {
                    var finalReviewRecords = records.FindAll(a => a.Status == "For Final Review");
                    applicantList.ItemsSource = finalReviewRecords;
                }
            }
            catch (Exception)
            {
            }
        }
    }
}