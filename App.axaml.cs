using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using HR_Applicant_System.Views;

namespace HR_Applicant_System
{
    public partial class App : Application
    {
        public override void Initialize() => AvaloniaXamlLoader.Load(this);

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Launches your synchronized login window first
                desktop.MainWindow = new StaffLoginView();
            }
            base.OnFrameworkInitializationCompleted();
        }
    }
}