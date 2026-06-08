using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;

namespace HR_Applicant_System.Views
{
    public class AdminLoginView : Window
    {
        private TextBox txtEmail;
        private TextBox txtPassword;

        public AdminLoginView()
        {
            Width = 480;
            Height = 380;
            Title = "Admin / Manager Login";

            txtEmail = new TextBox
            {
                Watermark = "Enter admin email"
            };

            txtPassword = new TextBox
            {
                Watermark = "Enter password",
                PasswordChar = '*'
            };

            Button btnLogin = new Button
            {
                Content = "Login",
                Height = 42,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };

            btnLogin.Click += Login_Click;

            Button btnBack = new Button
            {
                Content = "Back",
                Height = 38,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };

            btnBack.Click += Back_Click;

            StackPanel formPanel = new StackPanel
            {
                Spacing = 15,
                Children =
                {
                    new TextBlock
                    {
                        Text = "Admin / Manager Login",
                        FontSize = 26,
                        FontWeight = FontWeight.Bold,
                        HorizontalAlignment = HorizontalAlignment.Center
                    },

                    new TextBlock
                    {
                        Text = "Enter authorized admin credentials to access the dashboard.",
                        TextWrapping = TextWrapping.Wrap,
                        Foreground = Brushes.Gray,
                        HorizontalAlignment = HorizontalAlignment.Center
                    },

                    new TextBlock { Text = "Email Address" },
                    txtEmail,

                    new TextBlock { Text = "Password" },
                    txtPassword,

                    btnLogin,
                    btnBack
                }
            };

            Border card = new Border
            {
                Background = Brushes.White,
                CornerRadius = new CornerRadius(10),
                Padding = new Thickness(30),
                Width = 360,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Child = formPanel
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

        private void Login_Click(object? sender, RoutedEventArgs e)
        {
            string email = txtEmail.Text ?? "";
            string password = txtPassword.Text ?? "";

            // Hardcoded Admin/Manager credentials based on assigned module requirement
            if (email == "admin@gmail.com" && password == "admin123")
            {
                AdminView adminView = new AdminView();
                adminView.Show();
                this.Close();
            }
            else
            {
                ShowMessage("Invalid admin email or password.");
            }
        }

        private void Back_Click(object? sender, RoutedEventArgs e)
        {
            HR_Applicant_System.MainWindow mainWindow = new HR_Applicant_System.MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private async void ShowMessage(string message)
        {
            Window dialog = new Window
            {
                Width = 360,
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