﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System; // Added for Convert class
using Avalonia.Layout;
using Avalonia.Media;
using MySql.Data.MySqlClient;
using HR_Applicant_System.Models;

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
                PlaceholderText = "Enter admin email"
            };

            txtPassword = new TextBox
            {
                PlaceholderText = "Enter password",
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

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ShowMessage("Please enter both email and password.");
                return;
            }

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    // Query users table for Email/Password and ensure they have an Admin (1) or HR Manager (2) role
                    string query = "SELECT COUNT(*) FROM users WHERE Email = @Email AND Password = @Password AND RoleID IN (1, 2)";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Password", password);

                        int count = Convert.ToInt32(cmd.ExecuteScalar());

                        if (count > 0)
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
                }
            }
            catch (System.Exception ex)
            {
                ShowMessage($"Database connection error: {ex.Message}");
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