using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;

namespace HR_Applicant_System.Views
{
    public class ReportsView : Window
    {
        public ReportsView()
        {
            Width = 750;
            Height = 520;
            Title = "Reports";

            TextBlock title = new TextBlock
            {
                Text = "Recruitment Reports",
                FontSize = 26,
                FontWeight = FontWeight.Bold
            };

            TextBlock subtitle = new TextBlock
            {
                Text = "Admin/Manager can view recruitment summaries and generate basic reports.",
                Foreground = Brushes.Gray,
                TextWrapping = TextWrapping.Wrap
            };

            ListBox reportList = new ListBox
            {
                Height = 250,
                Items =
                {
                    "Applicant List Report",
                    "Active Job Vacancies Report",
                    "Interview Schedule Report",
                    "Accepted Applicants Report",
                    "Rejected Applicants Report",
                    "Missing Requirements Report"
                }
            };

            Button btnGenerate = new Button
            {
                Content = "Generate Selected Report",
                Height = 42,
                Width = 220
            };

            btnGenerate.Click += GenerateReport_Click;

            StackPanel contentPanel = new StackPanel
            {
                Spacing = 15,
                Children =
                {
                    title,
                    subtitle,
                    reportList,
                    btnGenerate
                }
            };

            Border card = new Border
            {
                Background = Brushes.White,
                CornerRadius = new CornerRadius(10),
                Padding = new Thickness(30),
                Margin = new Thickness(25),
                Child = contentPanel
            };

            Grid mainGrid = new Grid
            {
                Background = new SolidColorBrush(Color.Parse("#F3F4F6")),
                Children =
                {
                    card
                }
            };

            Content = mainGrid;
        }

        private void GenerateReport_Click(object? sender, RoutedEventArgs e)
        {
            ShowMessage("Report generation screen is working. Database report data will be added next.");
        }

        private async void ShowMessage(string message)
        {
            Window dialog = new Window
            {
                Width = 400,
                Height = 150,
                Title = "Message",
                Content = new TextBlock
                {
                    Text = message,
                    Margin = new Thickness(20),
                    TextWrapping = TextWrapping.Wrap,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                }
            };

            await dialog.ShowDialog(this);
        }
    }
}