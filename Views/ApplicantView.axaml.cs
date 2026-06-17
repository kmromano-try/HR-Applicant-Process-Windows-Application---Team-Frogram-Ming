using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Controls.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;
using HR_Applicant_System.Models;

namespace HR_Applicant_System.Views
{
    public partial class ApplicantView : Window
    {
        // Login/Registration UI elements
        private Border _loginCard;
        private TextBox _loginNameTextBox;
        private TextBox _loginEmailTextBox;
        private TextBox _loginPasswordTextBox; // Using ContactNumber as password for applicants

        // Application UI elements
        private Grid _applicationLayout;
        private TextBox txtFullName;
        private TextBox txtEmail;
        private TextBox txtContact;
        private ComboBox cbJobs;
        private ListBox _jobListBox; // To display open jobs
        private TextBox _applicantBioTextBox; // For applicant to update their bio

        private int _loggedInApplicantId = 0;
        private string _loggedInApplicantEmail = string.Empty;
        private string _loggedInApplicantFullName = string.Empty;

        public ApplicantView()
        {
            Width = 500;
            Height = 650;
            Title = "Job Application Portal";
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // --- Login/Registration Card ---
            _loginNameTextBox = new TextBox { PlaceholderText = "Enter your full name (registration only)" };
            _loginEmailTextBox = new TextBox { PlaceholderText = "Enter your email address" };
            _loginPasswordTextBox = new TextBox { PlaceholderText = "Enter your contact number (used as password)" };

            Button btnLogin = new Button
            {
                Content = "Login",
                Height = 45,
                Background = new SolidColorBrush(Color.Parse("#2563EB")),
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                FontWeight = FontWeight.Bold
            };
            btnLogin.Click += Login_Click;

            Button btnRegister = new Button
            {
                Content = "Register",
                Height = 35,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Background = new SolidColorBrush(Color.Parse("#4b5563")),
                Foreground = Brushes.White
            };
            btnRegister.Click += Register_Click;

            Button btnBackToMain = new Button
            {
                Content = "Back to Main Menu",
                Height = 35,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Background = new SolidColorBrush(Color.Parse("#4b5563")),
                Foreground = Brushes.White
            };
            btnBackToMain.Click += (s, e) =>
            {
                var mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            };

            StackPanel loginFormPanel = new StackPanel
            {
                Spacing = 15,
                Children =
                {
                    new TextBlock { Text = "Applicant Login / Register", FontSize = 28, FontWeight = FontWeight.Bold, HorizontalAlignment = HorizontalAlignment.Center, Foreground = Brushes.White },
                    new TextBlock { Text = "Login with your email and contact number, or register a new account.", Foreground = new SolidColorBrush(Color.Parse("#e0e0e0")), TextWrapping = TextWrapping.Wrap, HorizontalAlignment = HorizontalAlignment.Center },
                    new Separator { Height = 20, Opacity = 0 },
                    new TextBlock { Text = "Full Name (Register Only)", FontWeight = FontWeight.SemiBold, Foreground = Brushes.White },
                    _loginNameTextBox,
                    new TextBlock { Text = "Email Address", FontWeight = FontWeight.SemiBold, Foreground = Brushes.White },
                    _loginEmailTextBox,
                    new TextBlock { Text = "Contact Number (Password)", FontWeight = FontWeight.SemiBold, Foreground = Brushes.White },
                    _loginPasswordTextBox,
                    btnLogin,
                    btnRegister,
                    btnBackToMain
                }
            };

            _loginCard = new Border
            {
                Background = new SolidColorBrush(Color.Parse("#252525")),
                CornerRadius = new CornerRadius(12),
                Padding = new Thickness(35),
                Width = 420,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Child = loginFormPanel
            };

            // --- Application Layout (after login) ---
            txtFullName = new TextBox { PlaceholderText = "Enter your full name" };
            txtEmail = new TextBox { PlaceholderText = "Enter your email address" };
            txtContact = new TextBox { PlaceholderText = "Enter your contact number" };
            _applicantBioTextBox = new TextBox { PlaceholderText = "Tell us about yourself (Bio)", Height = 80, AcceptsReturn = true, TextWrapping = TextWrapping.Wrap };

            // Dropdown for Jobs
            cbJobs = new ComboBox
            {
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            _jobListBox = new ListBox
            {
                Height = 150,
                Margin = new Thickness(0, 10, 0, 10)
            };

            Button btnSubmit = new Button
            {
                Content = "Submit Application",
                Height = 45,
                Background = new SolidColorBrush(Color.Parse("#2563EB")),
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                FontWeight = FontWeight.Bold
            };
            btnSubmit.Click += Submit_Click;

            Button btnUpdateBio = new Button
            {
                Content = "Update Bio",
                Height = 35,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            btnUpdateBio.Click += UpdateBio_Click;

            _applicationLayout = new Grid
            {
                RowDefinitions = new RowDefinitions("Auto, Auto, *, Auto"),
                Background = new SolidColorBrush(Color.Parse("#1e1e1e")),
                IsVisible = false // Initially hidden
            };

            // Section 4: Logout Button
            Button btnLogout = new Button
            {
                Content = "Logout / Back to Main Menu",
                Height = 40,
                Background = new SolidColorBrush(Color.Parse("#ef4444")),
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = new Thickness(20, 10, 20, 20),
                FontWeight = FontWeight.Bold
            };
            btnLogout.Click += (s, e) =>
            {
                _loggedInApplicantId = 0;
                var mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            };
            _applicationLayout.Children.Add(btnLogout);
            Grid.SetRow(btnLogout, 3);

            // Section 1: Applicant Bio
            StackPanel bioPanel = new StackPanel
            {
                Spacing = 10,
                Margin = new Thickness(20),
                Children =
                {
                    new TextBlock { Text = "Your Profile", FontSize = 20, FontWeight = FontWeight.Bold, Foreground = Brushes.White },
                    new TextBlock { Text = "Full Name", FontWeight = FontWeight.SemiBold, Foreground = Brushes.White },
                    txtFullName,
                    new TextBlock { Text = "Email Address", FontWeight = FontWeight.SemiBold, Foreground = Brushes.White },
                    txtEmail,
                    new TextBlock { Text = "Contact Number", FontWeight = FontWeight.SemiBold, Foreground = Brushes.White },
                    txtContact,
                    new TextBlock { Text = "Your Bio/Description", FontWeight = FontWeight.SemiBold, Foreground = Brushes.White },
                    _applicantBioTextBox,
                    btnUpdateBio
                }
            };
            _applicationLayout.Children.Add(bioPanel);
            Grid.SetRow(bioPanel, 0);

            // Section 2: Open Job Positions (ListBox)
            StackPanel jobListPanel = new StackPanel
            {
                Spacing = 10,
                Margin = new Thickness(20),
                Children =
                {
                    new TextBlock { Text = "Open Job Positions", FontSize = 20, FontWeight = FontWeight.Bold, Foreground = Brushes.White },
                    _jobListBox
                }
            };
            _applicationLayout.Children.Add(jobListPanel);
            Grid.SetRow(jobListPanel, 1);

            // Section 3: Submit Application Form
            StackPanel applicationFormPanel = new StackPanel
            {
                Spacing = 15,
                Margin = new Thickness(20),
                Children =
                {
                    new TextBlock { Text = "Submit Application", FontSize = 20, FontWeight = FontWeight.Bold, Foreground = Brushes.White },
                    new TextBlock { Text = "Desired Position", FontWeight = FontWeight.SemiBold, Foreground = Brushes.White },
                    cbJobs,
                    btnSubmit
                }
            };
            _applicationLayout.Children.Add(applicationFormPanel);
            Grid.SetRow(applicationFormPanel, 2);

            // Main content grid to switch between login and application views
            var mainContentGrid = new Grid
            {
                Background = new SolidColorBrush(Color.Parse("#1e1e1e")),
                Children = { _loginCard, _applicationLayout }
            };
            Content = mainContentGrid;
        }

        public void Login_Click(object? sender, RoutedEventArgs e)
        {
            string email = _loginEmailTextBox.Text ?? "";
            string contactNumber = _loginPasswordTextBox.Text ?? ""; // Using ContactNumber as password

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(contactNumber))
            {
                ShowMessage("Login Error", "Please enter both email and contact number.");
                return;
            }

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = $"SELECT ApplicantID, FullName, Email, ContactNumber, Bio FROM {DatabaseHelper.ApplicantTable} WHERE Email = @Email AND ContactNumber = @ContactNumber";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@ContactNumber", contactNumber);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                _loggedInApplicantId = reader.GetInt32("ApplicantID");
                                _loggedInApplicantFullName = reader.GetString("FullName");
                                _loggedInApplicantEmail = reader.GetString("Email");
                                string bio = reader.IsDBNull(reader.GetOrdinal("Bio")) ? string.Empty : reader.GetString("Bio");

                                Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                                {
                                    txtFullName.Text = _loggedInApplicantFullName;
                                    txtEmail.Text = _loggedInApplicantEmail;
                                    txtContact.Text = contactNumber;
                                    _applicantBioTextBox.Text = bio;

                                    txtFullName.IsReadOnly = true;
                                    txtEmail.IsReadOnly = true;
                                    txtContact.IsReadOnly = true;

                                    _loginCard.IsVisible = false;
                                    _applicationLayout.IsVisible = true;
                                    LoadJobsIntoListBox();
                                });
                            }
                            else
                            {
                                ShowMessage("Login Failed", "Invalid email or contact number.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Database Error", $"Login failed: {ex.Message}");
            }
        }

        public void Register_Click(object? sender, RoutedEventArgs e)
        {
            string fullName = _loginNameTextBox.Text ?? "";
            string email = _loginEmailTextBox.Text ?? "";
            string contactNumber = _loginPasswordTextBox.Text ?? "";

            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(contactNumber))
            {
                ShowMessage("Registration Error", "Please enter full name, email and contact number for registration.");
                return;
            }

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    // Check if email already exists
                    string checkQuery = $"SELECT COUNT(*) FROM {DatabaseHelper.ApplicantTable} WHERE Email = @Email";
                    using (var checkCmd = new MySqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@Email", email);
                        if (Convert.ToInt32(checkCmd.ExecuteScalar()) > 0)
                        {
                            ShowMessage("Registration Failed", "An account with this email already exists.");
                            return;
                        }
                    }

                    // Register new applicant (FullName can be updated later)
                    string insertQuery = $"INSERT INTO {DatabaseHelper.ApplicantTable} (FullName, Email, ContactNumber) VALUES (@FullName, @Email, @ContactNumber)";
                    using (var insertCmd = new MySqlCommand(insertQuery, conn))
                    {
                        insertCmd.Parameters.AddWithValue("@FullName", fullName);
                        insertCmd.Parameters.AddWithValue("@Email", email);
                        insertCmd.Parameters.AddWithValue("@ContactNumber", contactNumber);
                        insertCmd.ExecuteNonQuery();
                    }
                    ShowMessage("Registration Success", "Account created successfully! You can now log in.");
                    // Automatically attempt to log in after registration
                    Login_Click(sender, e);
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Database Error", $"Registration failed: {ex.Message}");
            }
        }

        private void LoadJobsIntoListBox()
        {
            var jobRepo = new JobRepository();
            var availableJobs = jobRepo.GetAllJobs();

            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                _jobListBox.Items.Clear();
                cbJobs.Items.Clear();
                foreach (var job in availableJobs)
                {
                    _jobListBox.Items.Add($"Job #{job.VacancyID}: {job.JobTitle} ({job.Department}) - {job.Status}");
                    cbJobs.Items.Add(job.JobTitle);
                }
                if (cbJobs.Items.Count > 0)
                {
                    cbJobs.SelectedIndex = 0;
                }
                else
                {
                    _jobListBox.Items.Add("No open job vacancies found.");
                }
            });
        }

        public void Submit_Click(object? sender, RoutedEventArgs e)
        {
            if (_loggedInApplicantId == 0)
            {
                ShowMessage("Error", "Please log in first to submit an application.");
                return;
            }

            string selectedJobTitle = cbJobs.SelectedItem as string ?? "";

            if (string.IsNullOrEmpty(selectedJobTitle))
            {
                ShowMessage("Missing Information", "Please select a Position to apply for.");
                return;
            }

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    // FIXED: Changed JobID to VacancyID to match database schema tracking duplication
                    string checkExistingAppQuery = $"SELECT COUNT(*) FROM {DatabaseHelper.ApplicationTable} WHERE ApplicantID = @ApplicantID AND VacancyID = @VacancyID";
                    using (var checkCmd = new MySqlCommand(checkExistingAppQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@ApplicantID", _loggedInApplicantId);
                        checkCmd.Parameters.AddWithValue("@VacancyID", GetJobIdByTitle(selectedJobTitle));
                        if (Convert.ToInt32(checkCmd.ExecuteScalar()) > 0)
                        {
                            ShowMessage("Already Applied", "You have already applied for this position.");
                            return;
                        }
                    }

                        int jobId = GetJobIdByTitle(selectedJobTitle);

                        // FIXED: Changed JobID column to VacancyID column to prevent the popup crash layout 
                        string appSql = $"INSERT INTO {DatabaseHelper.ApplicationTable} (ApplicantID, VacancyID, Status) VALUES (@Aid, @Vid, 'Submitted')";
                        using (var cmdApp = new MySqlCommand(appSql, conn))
                        {
                            cmdApp.Parameters.AddWithValue("@Aid", _loggedInApplicantId);
                            cmdApp.Parameters.AddWithValue("@Vid", jobId);
                            cmdApp.ExecuteNonQuery();
                        }

                        ShowMessage("Success", "Application submitted! HR will review your profile shortly.");
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error", $"Could not submit application: {ex.Message}");
            }
        }

        public void UpdateBio_Click(object? sender, RoutedEventArgs e)
        {
            if (_loggedInApplicantId == 0)
            {
                ShowMessage("Error", "Please log in first to update your bio.");
                return;
            }

            string newBio = _applicantBioTextBox.Text ?? string.Empty;

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string updateQuery = $"UPDATE {DatabaseHelper.ApplicantTable} SET Bio = @Bio WHERE ApplicantID = @ApplicantID";
                    using (var cmd = new MySqlCommand(updateQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Bio", newBio);
                        cmd.Parameters.AddWithValue("@ApplicantID", _loggedInApplicantId);
                        cmd.ExecuteNonQuery();
                    }
                    ShowMessage("Success", "Your bio has been updated successfully.");
                }
            }
            catch (Exception ex) { ShowMessage("Database Error", $"Failed to update bio: {ex.Message}"); }
        }

        private int GetJobIdByTitle(string jobTitle)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                // FIXED: Changed JobID field query selector to VacancyID to match your structural tables
                string query = $"SELECT VacancyID FROM {DatabaseHelper.JobTable} WHERE JobTitle = @JobTitle";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@JobTitle", jobTitle);
                    object? result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        return Convert.ToInt32(result);
                    }
                    else
                    {
                        throw new Exception($"Job with title '{jobTitle}' not found.");
                    }
                }
            }
        }

        private async void ShowMessage(string title, string message)
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(async () =>
            {
                Window dialog = new Window { Width = 380, Height = 160, Title = title, WindowStartupLocation = WindowStartupLocation.CenterOwner, Content = new TextBlock { Text = message, Margin = new Thickness(25), TextWrapping = TextWrapping.Wrap, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center } };
                await dialog.ShowDialog(this);
            });
        }
    }
}