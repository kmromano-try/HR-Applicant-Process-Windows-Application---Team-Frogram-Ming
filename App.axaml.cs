using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace HR_Applicant_System;

public partial class App : Application
{
    public override void Initialize()
    {
        e,r3AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // FIX: Stops the app from crashing/killing itself when MainWindow closes
            desktop.ShutdownMode = ShutdownMode.OnLastWindowClose;
            desktop.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }
}