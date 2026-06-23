using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace HR_Applicant_System.Views
{
    public partial class StaffLoginView : Window
    {
        public StaffLoginView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}