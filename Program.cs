using Avalonia;
using System;
using System.Threading.Tasks;

namespace HR_Applicant_System;

class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        // 1. Catch standard thread crashes
        AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
        {
            Console.WriteLine("\n!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            Console.WriteLine("CRITICAL RUNTIME CRASH DETECTED:");
            Console.WriteLine(e.ExceptionObject?.ToString());
            Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!\n");
        };

        // 2. Catch background/database task crashes
        TaskScheduler.UnobservedTaskException += (sender, e) =>
        {
            Console.WriteLine("\n!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            Console.WriteLine("CRITICAL BACKGROUND TASK CRASH DETECTED:");
            Console.WriteLine(e.Exception?.ToString());
            Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!\n");
            e.SetObserved();
        };

        try
        {
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            Console.WriteLine("\n!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            Console.WriteLine("LIFETIME CRASH:");
            Console.WriteLine(ex.ToString());
            Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!\n");
        }
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
#if DEBUG
            .WithDeveloperTools()
#endif
            .WithInterFont()
            .LogToTrace();
}