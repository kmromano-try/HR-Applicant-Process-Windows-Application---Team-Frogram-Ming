using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using System;
using MySql.Data.MySqlClient;
using HR_Applicant_System.Models;
namespace HR_Applicant_System.Views
{
    public class CreateStaffView : Window
    {
        private TextBox txtStaffEmail;
        private TextBox txtTempPassword;
        private TextBox txtConfirmPassword;

        public CreateStaffView()
        {
            Width = 500;
            Height = 420;
            Title = "Create HR Staff Account";

            txtStaffEmail = new TextBox
            {
                PlaceholderText = "Enter staff email"
            };

            txtTempPassword = new TextBox
            {
                PlaceholderText = "Enter temporary password",
                PasswordChar = '*'
            };

            txtConfirmPassword = new TextBox
            {
                PlaceholderText = "Confirm password",
                PasswordChar = '*'
            };

            Button btnCreate = new Button
            {
                Content = "Create Staff Account",
                Height = 42,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };

            btnCreate.Click += CreateStaffAccount_Click;

            StackPanel formPanel = new StackPanel
            {
                Spacing = 15,
                Children =
                {
                    new TextBlock
                    {
                        Text = "Create HR Staff Account",
                        FontSize = 24,
                        FontWeight = FontWeight.Bold,
                        Foreground = Brushes.White
                    },

                    new TextBlock
                    {
                        Text = "Admin/Manager can create a staff login account. Staff profile details can be completed later.",
                        TextWrapping = TextWrapping.Wrap,
                        Foreground = new SolidColorBrush(Color.Parse("#e0e0e0"))
                    },

                    new TextBlock { Text = "Email Address", Foreground = Brushes.White },
                    txtStaffEmail,

                    new TextBlock { Text = "Temporary Password", Foreground = Brushes.White },
                    txtTempPassword,

                    new TextBlock { Text = "Confirm Password", Foreground = Brushes.White },
                    txtConfirmPassword,

                    btnCreate
                }
            };

            Border card = new Border
            {
                Background = new SolidColorBrush(Color.Parse("#252525")),
                CornerRadius = new CornerRadius(10),
                Padding = new Thickness(30),
                Width = 400,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Child = formPanel
            };

            Grid mainGrid = new Grid
            {
                Background = new SolidColorBrush(Color.Parse("#1e1e1e")),
                Children =
                {
                    card
                }
            };

            Content = mainGrid;
        }

        private void CreateStaffAccount_Click(object? sender, RoutedEventArgs e)
        {
            string email = txtStaffEmail.Text ?? "";
            string password = txtTempPassword.Text ?? "";
            string confirmPassword = txtConfirmPassword.Text ?? "";

            if (email.Trim() == "" || password.Trim() == "" || confirmPassword.Trim() == "")
            {
                ShowMessage("Please fill out all fields.");
                return;
            }

            if (password != confirmPassword)
            {
                ShowMessage("Password and confirm password do not match.");
                return;
            }

            try
            {
                using (MySqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    string checkQuery = $"SELECT COUNT(*) FROM {DatabaseHelper.StaffTable} WHERE Email = @Email";
                    using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@Email", email);

                        int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                        if (count > 0)
                        {
                            ShowMessage("This email is already registered.");
                            return;
                        }
                    }

                    string insertQuery = $@"INSERT INTO {DatabaseHelper.StaffTable} 
                               (Email, Password) 
                               VALUES 
                               (@Email, @Password)";

                    using (MySqlCommand cmd = new MySqlCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Password", password);

                        cmd.ExecuteNonQuery();
                    }

                    ShowMessage("HR Staff account created successfully.");

                    txtStaffEmail.Text = "";
                    txtTempPassword.Text = "";
                    txtConfirmPassword.Text = "";
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Database error: " + ex.Message);
            }
        }

        private async void ShowMessage(string message)
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(async () =>
            {
                Window dialog = new Window
                {
                    Width = 350,
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
            });
        }
    }
}