using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using Avalonia.Layout;
using Avalonia.Media;
using MySql.Data.MySqlClient;
using HR_Applicant_System.Models;
using HR_Applicant_System.ViewModels;
using Avalonia.Threading;

namespace HR_Applicant_System.Views
{
    public class StaffLoginView : Window
    {
        private TextBox txtEmail;
        private TextBox txtPassword;

        public StaffLoginView()
        {
            Width = 480;
            Height = 400;
            Title = "HR Staff Login";
            Background = Brush.Parse("#1e1e1e");
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            txtEmail = new TextBox { PlaceholderText = "Enter staff email" };
            txtPassword = new TextBox { PlaceholderText = "Enter password", PasswordChar = '*' };

            Button btnLogin = new Button
            {
                Content = "Login to Dashboard",
                Height = 42,
                Background = Brush.Parse("#198754"),
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                FontWeight = FontWeight.Bold
            };
            btnLogin.Click += Login_Click;

            Button btnBack = new Button
            {
                Content = "Back to Main Menu",
                Height = 38,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            btnBack.Click += (s, e) => {
                new MainWindow().Show();
                this.Close();
            };

            StackPanel formPanel = new StackPanel
            {
                Spacing = 15,
                Children =
                {
                    new TextBlock { Text = "HR Staff Portal", FontSize = 26, FontWeight = FontWeight.Bold, HorizontalAlignment = HorizontalAlignment.Center, Foreground = Brushes.White },
                    new TextBlock { Text = "Access the evaluation pipeline and manage applicants.", TextWrapping = TextWrapping.Wrap, Foreground = Brush.Parse("#e0e0e0"), HorizontalAlignment = HorizontalAlignment.Center },
                    new TextBlock { Text = "Email Address", Foreground = Brushes.White, FontWeight = FontWeight.SemiBold },
                    txtEmail,
                    new TextBlock { Text = "Password", Foreground = Brushes.White, FontWeight = FontWeight.SemiBold },
                    txtPassword,
                    btnLogin,
                    btnBack
                }
            };

            Content = new Border
            {
                Background = Brush.Parse("#252525"),
                CornerRadius = new CornerRadius(10),
                Padding = new Thickness(30),
                Width = 380,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Child = formPanel
            };
        }

        private void Login_Click(object? sender, RoutedEventArgs e)
        {
            string email = txtEmail.Text ?? "";
            string password = txtPassword.Text ?? "";

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ShowMessage("Login Failed", "Please provide both email and password.");
                return;
            }

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = $"SELECT FullName, Bio FROM {DatabaseHelper.StaffTable} WHERE Email = @Email AND Password = @Password";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Password", password);
                        
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string dbName = reader.IsDBNull(reader.GetOrdinal("FullName")) ? "" : reader.GetString("FullName");
                                string dbBio = reader.IsDBNull(reader.GetOrdinal("Bio")) ? "" : reader.GetString("Bio");
                                
                                var viewModel = new ApplicantListViewModel(email)
                                {
                                    FullName = dbName,
                                    Bio = dbBio
                                };

                                var staffWindow = new StaffView
                                {
                                    DataContext = viewModel
                                };
                                
                                this.Hide();
                                staffWindow.Show();
                                Avalonia.Threading.Dispatcher.UIThread.Post(this.Close, DispatcherPriority.ApplicationIdle);
                            }
                            else { ShowMessage("Login Failed", "Invalid credentials."); }
                        }
                    }
                }
            }
            catch (Exception ex) 
            { 
                ShowMessage("Error", ex.Message); 
            }
        }

        private void ShowMessage(string title, string message)
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(() => {
                var dialog = new Window { Width = 350, Height = 150, Title = title, Background = Brush.Parse("#1e1e1e"), Content = new TextBlock { Text = message, Foreground = Brushes.White, Margin = new Thickness(20), TextWrapping = TextWrapping.Wrap, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center } };
                dialog.ShowDialog(this);
            });
        }
    }
}