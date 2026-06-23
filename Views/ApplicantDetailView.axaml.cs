using Avalonia.Controls;

namespace HR_Applicant_System.Views
{
    public partial class ApplicantDetailView : Window
    {
        public ApplicantDetailView()
        {
            InitializeComponent();
        }

        public ApplicantDetailView(int applicationId) : this()
        {
            // Ready for loading applicant-specific data fields later
        }
    }
}