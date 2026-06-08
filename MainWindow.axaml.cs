using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace AvaloniaApplication1;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private async void Login_Click(object? sender, RoutedEventArgs e)
    {
        await ShowMessage("Success", "Login button clicked!");
    }

    private async void Register_Click(object? sender, RoutedEventArgs e)
    {
        await ShowMessage("Register", "Create Account button clicked!");
    }

    private async Task ShowMessage(string title, string message)
    {
        var dialog = new Window
        {
            Title = title,
            Width = 350,
            Height = 180,
            Content = new TextBlock
            {
                Text = message,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
            }
        };

        await dialog.ShowDialog(this);
    }
}