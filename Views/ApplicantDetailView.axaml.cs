using Avalonia.Controls;
using Avalonia.Markup.Xaml;

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
            // LoadData(applicationId); 
        }
    }
}