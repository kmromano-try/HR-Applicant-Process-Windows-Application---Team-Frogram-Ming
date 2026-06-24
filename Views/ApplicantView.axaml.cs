using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Platform.Storage; // Required for Avalonia v11 StorageProvider file access
using System;
using System.Collections.Generic;
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
        private TextBox _loginPasswordTextBox; 

        // Application UI elements
        private Grid _applicationLayout;
        private TextBox txtFullName;
        private TextBox txtEmail;
        private TextBox txtContact;
        private TextBox txtAddress;
        private TextBox txtEducation;
        private TextBox txtSkills;
        private TextBox txtWorkExperience;
        private TextBox _applicantBioTextBox;
        
        // Security UI
        private TextBox txtOldPassword;
        private TextBox txtNewPassword;

        // Document UI
        private TextBox txtResumePath;
        private TextBox txtIdPath;
        private TextBox txtTranscriptPath;
        private TextBox txtCertificatePath;
        private TextBlock lblDocStatus;

        private ComboBox cbJobs;
        private ListBox _activeJobsList;
        private ListBox _myApplicationsList;
        private ListBox _closedJobsList;
        
        private int _loggedInApplicantId = 0;
        private string _loggedInApplicantEmail = string.Empty;
        private string _loggedInApplicantFullName = string.Empty;

        // FIX: Parameterless constructor added to resolve Avalonia warning AVLN3001
        public ApplicantView() : this(0)
        {
        }

        public ApplicantView(int applicantId)
        {
            InitializeComponent();
            _loggedInApplicantId = applicantId;
           
            Width = 600;
            Height = 700;
            Title = "Job Application Portal";
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // --- Login/Registration Panel ---
            _loginNameTextBox = new TextBox { PlaceholderText = "Enter your full name (registration only)" };
            _loginEmailTextBox = new TextBox { PlaceholderText = "Enter your email address" };
            _loginPasswordTextBox = new TextBox { PlaceholderText = "Enter your password", PasswordChar = '*' };

            Button btnLogin = new Button
            {
                Content = "Login", Height = 45,
                Background = new SolidColorBrush(Color.Parse("#2563EB")), Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Stretch, FontWeight = FontWeight.Bold
            };
            btnLogin.Click += Login_Click;

            Button btnRegister = new Button
            {
                Content = "Register New Account", Height = 35,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Background = new SolidColorBrush(Color.Parse("#4b5563")), Foreground = Brushes.White
            };
            btnRegister.Click += Register_Click;

            Button btnBackToMain = new Button
            {
                Content = "Back to Main Menu", Height = 35,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Background = new SolidColorBrush(Color.Parse("#4b5563")), Foreground = Brushes.White
            };
            btnBackToMain.Click += (s, e) => { new MainWindow().Show(); this.Close(); };

            StackPanel loginFormPanel = new StackPanel
            {
                Spacing = 12,
                Children =
                {
                    new TextBlock { Text = "Applicant Portal", FontSize = 28, FontWeight = FontWeight.Bold, HorizontalAlignment = HorizontalAlignment.Center, Foreground = Brushes.White },
                    new TextBlock { Text = "Access your application profile, track review statuses, and upload documentation.", Foreground = new SolidColorBrush(Color.Parse("#e0e0e0")), TextWrapping = TextWrapping.Wrap, HorizontalAlignment = HorizontalAlignment.Center },
                    new Separator { Height = 10, Opacity = 0 },
                    new TextBlock { Text = "Full Name (Register Only)", Foreground = Brushes.White }, _loginNameTextBox,
                    new TextBlock { Text = "Email Address", Foreground = Brushes.White }, _loginEmailTextBox,
                    new TextBlock { Text = "Password", Foreground = Brushes.White }, _loginPasswordTextBox,
                    btnLogin, btnRegister, btnBackToMain
                }
            };

            _loginCard = new Border
            {
                Background = new SolidColorBrush(Color.Parse("#252525")), CornerRadius = new CornerRadius(12),
                Padding = new Thickness(35), Width = 420, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center,
                Child = loginFormPanel
            };

            // --- Profile Tab Setup ---
            txtFullName = new TextBox { IsReadOnly = false };
            txtEmail = new TextBox { IsReadOnly = true };
            txtContact = new TextBox();
            txtAddress = new TextBox();
            txtEducation = new TextBox { AcceptsReturn = true, Height = 60 };
            txtSkills = new TextBox { AcceptsReturn = true, Height = 60 };
            txtWorkExperience = new TextBox { AcceptsReturn = true, Height = 60 };
            _applicantBioTextBox = new TextBox { AcceptsReturn = true, Height = 60 };

            Button btnSaveProfile = new Button { Content = "Save / Update Profile Details", Height = 40, Background = new SolidColorBrush(Color.Parse("#16A34A")), Foreground = Brushes.White, HorizontalAlignment = HorizontalAlignment.Stretch };
            btnSaveProfile.Click += UpdateProfile_Click;

            txtOldPassword = new TextBox { PasswordChar = '*', PlaceholderText = "Current Password" };
            txtNewPassword = new TextBox { PasswordChar = '*', PlaceholderText = "New Password" };
            Button btnChangePass = new Button { Content = "Update Security Password", Height = 35, Background = new SolidColorBrush(Color.Parse("#4B5563")), Foreground = Brushes.White };
            btnChangePass.Click += ChangePassword_Click;

            ScrollViewer profileScrollViewer = new ScrollViewer
            {
                Content = new StackPanel
                {
                    Spacing = 10, Margin = new Thickness(10), 
                    Children =
                    {
                        new TextBlock { Text = "My Personal Information", FontSize = 18, FontWeight = FontWeight.Bold, Foreground = Brushes.White },
                        new TextBlock { Text = "Full Name", Foreground = Brushes.Gray }, txtFullName,
                        new TextBlock { Text = "Email", Foreground = Brushes.Gray }, txtEmail,
                        new TextBlock { Text = "Contact Number", Foreground = Brushes.Gray }, txtContact,
                        new TextBlock { Text = "Current Home Address", Foreground = Brushes.Gray }, txtAddress,
                        new TextBlock { Text = "Professional Summary / Bio", Foreground = Brushes.Gray }, _applicantBioTextBox,
                        new Separator { Height = 10 },
                        new TextBlock { Text = "Qualifications & Background", FontSize = 18, FontWeight = FontWeight.Bold, Foreground = Brushes.White },
                        new TextBlock { Text = "Education History", Foreground = Brushes.Gray }, txtEducation,
                        new TextBlock { Text = "Skills Registry", Foreground = Brushes.Gray }, txtSkills,
                        new TextBlock { Text = "Work Experience", Foreground = Brushes.Gray }, txtWorkExperience,
                        btnSaveProfile,
                        new Separator { Height = 15 },
                        new TextBlock { Text = "Account Security Settings", FontSize = 16, FontWeight = FontWeight.Bold, Foreground = Brushes.White },
                        txtOldPassword, txtNewPassword, btnChangePass
                    }
                }
            };

            // --- Active Vacancies Tab Setup ---
            _activeJobsList = new ListBox { Height = 220, Margin = new Thickness(0, 5, 0, 5) };
            cbJobs = new ComboBox { HorizontalAlignment = HorizontalAlignment.Stretch };
            Button btnCreateDraft = new Button
            {
                Content = "Initialize Application Draft", Height = 45,
                Background = new SolidColorBrush(Color.Parse("#2563EB")), Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Stretch, FontWeight = FontWeight.Bold
            };
            btnCreateDraft.Click += CreateDraft_Click;

            StackPanel vacanciesPanel = new StackPanel
            {
                Spacing = 10, Margin = new Thickness(15), 
                Children =
                {
                    new TextBlock { Text = "Open Job Vacancies", FontSize = 20, FontWeight = FontWeight.Bold, Foreground = Brushes.White },
                    _activeJobsList,
                    new TextBlock { Text = "Select Intended Position for Submission Pipeline:", Foreground = Brushes.White },
                    cbJobs, btnCreateDraft
                }
            };

            // --- My Applications Ledger Tab Setup ---
            _myApplicationsList = new ListBox { Margin = new Thickness(10) };

            // --- Documents Management Tab Setup ---
            txtResumePath = new TextBox { PlaceholderText = "No file selected...", IsReadOnly = true };
            txtIdPath = new TextBox { PlaceholderText = "No file selected...", IsReadOnly = true };
            txtTranscriptPath = new TextBox { PlaceholderText = "No file selected...", IsReadOnly = true };
            txtCertificatePath = new TextBox { PlaceholderText = "No file selected...", IsReadOnly = true };
            lblDocStatus = new TextBlock { Text = "Status Check: Unverified Data", Foreground = Brushes.Yellow, FontWeight = FontWeight.Bold, Margin = new Thickness(5) };

            Button btnBrowseResume = new Button { Content = "Upload Resume File" };
            btnBrowseResume.Click += async (s, e) => { var path = await BrowseFileAsync(); if(path != null) { txtResumePath.Text = path; SaveDocPath("Resume", path); } };

            Button btnBrowseId = new Button { Content = "Upload Official ID" };
            btnBrowseId.Click += async (s, e) => { var path = await BrowseFileAsync(); if(path != null) { txtIdPath.Text = path; SaveDocPath("ID", path); } };

            Button btnBrowseTranscript = new Button { Content = "Upload Transcript" };
            btnBrowseTranscript.Click += async (s, e) => { var path = await BrowseFileAsync(); if(path != null) { txtTranscriptPath.Text = path; SaveDocPath("Transcript", path); } };

            Button btnBrowseCert = new Button { Content = "Upload Certificate" };
            btnBrowseCert.Click += async (s, e) => { var path = await BrowseFileAsync(); if(path != null) { txtCertificatePath.Text = path; SaveDocPath("Certificate", path); } };

            StackPanel documentsPanel = new StackPanel
            {
                Spacing = 12, Margin = new Thickness(15), 
                Children =
                {
                    new TextBlock { Text = "Document Tracking Center", FontSize = 20, FontWeight = FontWeight.Bold, Foreground = Brushes.White },
                    lblDocStatus,
                    new TextBlock { Text = "Required: Core Work History Resume", Foreground = Brushes.White },
                    new Grid { ColumnDefinitions = new ColumnDefinitions("*, Auto"), Children = { txtResumePath, btnBrowseResume } },
                    new TextBlock { Text = "Required: Valid Government Identity Card", Foreground = Brushes.White },
                    new Grid { ColumnDefinitions = new ColumnDefinitions("*, Auto"), Children = { txtIdPath, btnBrowseId } },
                    new TextBlock { Text = "Optional: Academic Transcript Records", Foreground = Brushes.White },
                    new Grid { ColumnDefinitions = new ColumnDefinitions("*, Auto"), Children = { txtTranscriptPath, btnBrowseTranscript } },
                    new TextBlock { Text = "Optional: Accreditations & Certifications", Foreground = Brushes.White },
                    new Grid { ColumnDefinitions = new ColumnDefinitions("*, Auto"), Children = { txtCertificatePath, btnBrowseCert } }
                }
            };
            Grid.SetColumn(btnBrowseResume, 1); Grid.SetColumn(btnBrowseId, 1); Grid.SetColumn(btnBrowseTranscript, 1); Grid.SetColumn(btnBrowseCert, 1);

            // --- Closed Jobs Tab Setup ---
            _closedJobsList = new ListBox { Margin = new Thickness(10) };

            // --- Main Workspace Assemblies ---
            _applicationLayout = new Grid { Background = new SolidColorBrush(Color.Parse("#1e1e1e")), IsVisible = false, RowDefinitions = new RowDefinitions("*, Auto") };
            Button btnLogout = new Button { Content = "Exit Portal Framework", Height = 40, Background = new SolidColorBrush(Color.Parse("#ef4444")), Foreground = Brushes.White, HorizontalAlignment = HorizontalAlignment.Stretch, Margin = new Thickness(10) };
            btnLogout.Click += (s, e) => { _loggedInApplicantId = 0; new MainWindow().Show(); this.Close(); };

            TabControl tabControl = new TabControl
            {
                Margin = new Thickness(5),
                Items =
                {
                    new TabItem { Header = "My Profile", Content = profileScrollViewer },
                    new TabItem { Header = "Active Vacancies", Content = vacanciesPanel },
                    new TabItem { Header = "My Applications", Content = _myApplicationsList },
                    new TabItem { Header = "My Documents", Content = documentsPanel },
                    new TabItem { Header = "Closed Jobs", Content = _closedJobsList }
                }
            };

            Grid.SetRow(tabControl, 0); Grid.SetRow(btnLogout, 1);
            _applicationLayout.Children.Add(tabControl); _applicationLayout.Children.Add(btnLogout);

            Content = new Grid { Background = new SolidColorBrush(Color.Parse("#1e1e1e")), Children = { _loginCard, _applicationLayout } };

            if (_loggedInApplicantId > 0) { TriggerPostLoginState(); }
        }

        private void TriggerPostLoginState()
        {
            _loginCard.IsVisible = false;
            _applicationLayout.IsVisible = true;
            LoadApplicantProfileData();
            LoadJobsIntoListBox();
            LoadMyApplications();
            LoadClosedJobs();
            LoadSavedDocumentPaths();
            ApplyLockState();
        }

        public void Login_Click(object? sender, RoutedEventArgs e)
        {
            string email = _loginEmailTextBox.Text ?? "";
            string password = _loginPasswordTextBox.Text ?? "";

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ShowMessage("Field Deficit", "Ensure account email mapping strings and safety passwords are filled.");
                return;
            }

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = $"SELECT ApplicantID, FullName, Email FROM {DatabaseHelper.ApplicantTable} WHERE Email = @Email AND Password = @Password";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Password", password);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                _loggedInApplicantId = reader.GetInt32("ApplicantID");
                                _loggedInApplicantFullName = reader.GetString("FullName");
                                _loggedInApplicantEmail = reader.GetString("Email");

                                Avalonia.Threading.Dispatcher.UIThread.Post(() => { TriggerPostLoginState(); });
                            }
                            else { ShowMessage("Access Denied", "No record entry matches provided credentials."); }
                        }
                    }
                }
            }
            catch (Exception ex) { ShowMessage("Engine Failure", ex.Message); }
        }

        public void Register_Click(object? sender, RoutedEventArgs e)
        {
            string fullName = _loginNameTextBox.Text ?? "";
            string email = _loginEmailTextBox.Text ?? "";
            string password = _loginPasswordTextBox.Text ?? "";

            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ShowMessage("Validation Failure", "All validation registers are required for initializing profiles.");
                return;
            }

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string checkQuery = $"SELECT COUNT(*) FROM {DatabaseHelper.ApplicantTable} WHERE Email = @Email";
                    using (var checkCmd = new MySqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@Email", email);
                        if (Convert.ToInt32(checkCmd.ExecuteScalar()) > 0)
                        {
                            ShowMessage("Conflict Check", "An account registration profile already targets this email tracking loop.");
                            return;
                        }
                    }

                    string insertQuery = $"INSERT INTO {DatabaseHelper.ApplicantTable} (FullName, Email, Password, ContactNumber) VALUES (@FullName, @Email, @Password, '')";
                    using (var insertCmd = new MySqlCommand(insertQuery, conn))
                    {
                        insertCmd.Parameters.AddWithValue("@FullName", fullName);
                        insertCmd.Parameters.AddWithValue("@Email", email);
                        insertCmd.Parameters.AddWithValue("@Password", password);
                        insertCmd.ExecuteNonQuery();
                    }
                    ShowMessage("Profile Created", "Credentials recorded successfully.");
                    Login_Click(sender, e);
                }
            }
            catch (Exception ex) { ShowMessage("Engine Error", ex.Message); }
        }

        private void LoadApplicantProfileData()
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = $"SELECT FullName, Email, ContactNumber, Address, Bio, Education, Skills, WorkExperience FROM {DatabaseHelper.ApplicantTable} WHERE ApplicantID = @Id";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", _loggedInApplicantId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtFullName.Text = reader["FullName"].ToString();
                                txtEmail.Text = reader["Email"].ToString();
                                txtContact.Text = reader["ContactNumber"].ToString();
                                txtAddress.Text = reader["Address"].ToString();
                                _applicantBioTextBox.Text = reader["Bio"].ToString();
                                txtEducation.Text = reader["Education"].ToString();
                                txtSkills.Text = reader["Skills"].ToString();
                                txtWorkExperience.Text = reader["WorkExperience"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine(ex.Message); }
        }

        private void UpdateProfile_Click(object? sender, RoutedEventArgs e)
{
    if (IsApplicationLocked()) { ShowMessage("Action Locked", "Changes disabled."); return; }

    try
    {
        using (var conn = DatabaseHelper.GetConnection())
        {
            conn.Open();
            // ADDED: FullName=@Name to the query
            string query = $"UPDATE {DatabaseHelper.ApplicantTable} SET FullName=@Name, ContactNumber=@Contact, Address=@Address, Bio=@Bio, Education=@Edu, Skills=@Skills, WorkExperience=@Work WHERE ApplicantID=@Id";
            using (var cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Name", txtFullName.Text ?? ""); // ADDED parameter
                cmd.Parameters.AddWithValue("@Contact", txtContact.Text ?? "");
                cmd.Parameters.AddWithValue("@Address", txtAddress.Text ?? "");
                cmd.Parameters.AddWithValue("@Bio", _applicantBioTextBox.Text ?? "");
                cmd.Parameters.AddWithValue("@Edu", txtEducation.Text ?? "");
                cmd.Parameters.AddWithValue("@Skills", txtSkills.Text ?? "");
                cmd.Parameters.AddWithValue("@Work", txtWorkExperience.Text ?? "");
                cmd.Parameters.AddWithValue("@Id", _loggedInApplicantId);
                cmd.ExecuteNonQuery();
            }
            ShowMessage("Success", "Profile data synchronized.");
        }
    }
    catch (Exception ex) { ShowMessage("Write Fail", ex.Message); }
}

        private void ChangePassword_Click(object? sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtOldPassword.Text) || string.IsNullOrWhiteSpace(txtNewPassword.Text)) { ShowMessage("Error", "Fill password targets."); return; }

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string verifyQuery = $"SELECT COUNT(*) FROM {DatabaseHelper.ApplicantTable} WHERE ApplicantID=@Id AND Password=@Old";
                    using (var vCmd = new MySqlCommand(verifyQuery, conn))
                    {
                        vCmd.Parameters.AddWithValue("@Id", _loggedInApplicantId);
                        vCmd.Parameters.AddWithValue("@Old", txtOldPassword.Text);
                        if (Convert.ToInt32(vCmd.ExecuteScalar()) == 0) { ShowMessage("Failed", "Old password text match validation failed."); return; }
                    }

                    string updateQuery = $"UPDATE {DatabaseHelper.ApplicantTable} SET Password=@New WHERE ApplicantID=@Id";
                    using (var uCmd = new MySqlCommand(updateQuery, conn))
                    {
                        uCmd.Parameters.AddWithValue("@New", txtNewPassword.Text);
                        uCmd.Parameters.AddWithValue("@Id", _loggedInApplicantId);
                        uCmd.ExecuteNonQuery();
                    }
                    ShowMessage("Confirmed", "Security layer password string changed.");
                    txtOldPassword.Text = ""; txtNewPassword.Text = "";
                }
            }
            catch (Exception ex) { ShowMessage("Security Error", ex.Message); }
        }

        private void LoadJobsIntoListBox()
        {
            var jobRepo = new JobRepository();
            var availableJobs = jobRepo.GetAllJobs();

            _activeJobsList.Items.Clear(); cbJobs.Items.Clear();

            foreach (var job in availableJobs)
            {
                if (job.Status.Equals("Active", StringComparison.OrdinalIgnoreCase))
                {
                    _activeJobsList.Items.Add($"Job #{job.VacancyID}: {job.JobTitle} ({job.Department})");
                    cbJobs.Items.Add(job.JobTitle);
                }
            }
        }

        private int GetJobIdByTitle(string title)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = $"SELECT VacancyID FROM {DatabaseHelper.JobTable} WHERE JobTitle = @Title LIMIT 1";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Title", title);
                        var obj = cmd.ExecuteScalar();
                        return obj != null ? Convert.ToInt32(obj) : 0;
                    }
                }
            }
            catch { return 0; }
        }

        private void CreateDraft_Click(object? sender, RoutedEventArgs e)
        {
            string selectedJobTitle = cbJobs.SelectedItem as string ?? "";
            if (string.IsNullOrEmpty(selectedJobTitle)) { ShowMessage("Incomplete", "Select an operational vacancy track."); return; }

            int vacancyId = GetJobIdByTitle(selectedJobTitle);

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string checkQuery = $"SELECT COUNT(*) FROM {DatabaseHelper.ApplicationTable} WHERE ApplicantID=@Aid AND VacancyID=@Vid";
                    using (var cCmd = new MySqlCommand(checkQuery, conn))
                    {
                        cCmd.Parameters.AddWithValue("@Aid", _loggedInApplicantId);
                        cCmd.Parameters.AddWithValue("@Vid", vacancyId);
                        if (Convert.ToInt32(cCmd.ExecuteScalar()) > 0) { ShowMessage("Duplicate Warning", "An open deployment registry record already covers this workspace."); return; }
                    }

                    string insertQuery = $"INSERT INTO {DatabaseHelper.ApplicationTable} (ApplicantID, VacancyID, Status) VALUES (@Aid, @Vid, 'Draft')";
                    using (var iCmd = new MySqlCommand(insertQuery, conn))
                    {
                        iCmd.Parameters.AddWithValue("@Aid", _loggedInApplicantId);
                        iCmd.Parameters.AddWithValue("@Vid", vacancyId);
                        iCmd.ExecuteNonQuery();
                    }
                    ShowMessage("Draft Initialized", "Application generated as an editable Local Draft. Finalize it in 'My Applications'.");
                    LoadMyApplications();
                }
            }
            catch (Exception ex) { ShowMessage("Draft Failure", ex.Message); }
        }

        private void LoadMyApplications()
        {
            _myApplicationsList.Items.Clear();

            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = $"SELECT a.ApplicationID, j.JobTitle, a.Status, a.StaffFeedback FROM {DatabaseHelper.ApplicationTable} a INNER JOIN {DatabaseHelper.JobTable} j ON a.VacancyID = j.VacancyID WHERE a.ApplicantID = @Id";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", _loggedInApplicantId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int appId = reader.GetInt32("ApplicationID");
                            string title = reader["JobTitle"].ToString() ?? "";
                            string status = reader["Status"].ToString() ?? "";
                            string feedback = reader["StaffFeedback"].ToString() ?? "";

                            var panel = new StackPanel { Spacing = 5, Margin = new Thickness(5) };
                            panel.Children.Add(new TextBlock { Text = $"{title} — Status Layout: [{status.ToUpper()}]", Foreground = Brushes.White, FontWeight = FontWeight.Bold });
                            if(!string.IsNullOrEmpty(feedback)) panel.Children.Add(new TextBlock { Text = $"HR Loop Remarks: {feedback}", Foreground = Brushes.LightGray, FontSize = 12 });

                            if (status.Equals("Draft", StringComparison.OrdinalIgnoreCase))
                            {
                                var btnSubmitFinal = new Button { Content = "Commit & Submit to HR", Tag = appId, Background = Brushes.DarkGreen, Foreground = Brushes.White, Margin = new Thickness(0,5,0,0) };
                                btnSubmitFinal.Click += FinalizeSubmission_Click;
                                panel.Children.Add(btnSubmitFinal);
                            }

                            var container = new Border { Padding = new Thickness(12), CornerRadius = new CornerRadius(6), Margin = new Thickness(0,4,0,4), Child = panel };
                            
                            if (status.Equals("Rejected", StringComparison.OrdinalIgnoreCase)) container.Background = new SolidColorBrush(Color.Parse("#DC2626"));
                            else if (status.Equals("Interview Scheduled", StringComparison.OrdinalIgnoreCase)) container.Background = new SolidColorBrush(Color.Parse("#2563EB"));
                            else if (status.Equals("Accepted", StringComparison.OrdinalIgnoreCase)) container.Background = new SolidColorBrush(Color.Parse("#16A34A"));
                            else if (status.Equals("Draft", StringComparison.OrdinalIgnoreCase)) container.Background = new SolidColorBrush(Color.Parse("#4B5563"));
                            else container.Background = new SolidColorBrush(Color.Parse("#374151"));

                            _myApplicationsList.Items.Add(container);
                        }
                    }
                }
            }
        }

        private void FinalizeSubmission_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int appId)
            {
                if (string.IsNullOrEmpty(txtResumePath.Text)) { ShowMessage("Blocker", "Core work history documentation registry requires a matching file upload link path."); return; }

                try
                {
                    using (var conn = DatabaseHelper.GetConnection())
                    {
                        conn.Open();
                        string query = $"UPDATE {DatabaseHelper.ApplicationTable} SET Status='Submitted' WHERE ApplicationID=@Id";
                        using (var cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@Id", appId);
                            cmd.ExecuteNonQuery();
                        }
                        ShowMessage("Success", "Application locked and submitted into the HR evaluation screening track.");
                        LoadMyApplications();
                        ApplyLockState();
                    }
                }
                catch (Exception ex) { ShowMessage("Error", ex.Message); }
            }
        }

        private async System.Threading.Tasks.Task<string?> BrowseFileAsync()
{
    var topLevel = TopLevel.GetTopLevel(this);
    if (topLevel == null) return null;

    // By removing the FileTypeFilter property, the picker will default 
    // to showing all files in the directory without graying anything out.
    var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
    {
        Title = "Select Registry Attachment File Source",
        AllowMultiple = false
        // REMOVED: FileTypeFilter
    });

    if (files.Count > 0)
    {
        string selectedPath = files[0].Path.LocalPath;
        
        // Manual Validation (Check here instead of the picker filter)
        string ext = System.IO.Path.GetExtension(selectedPath).ToLower();
        if (ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".pdf" || ext == ".docx")
        {
            return selectedPath;
        }
        else
        {
            ShowMessage("Unsupported Format", "Please select a valid image or document file.");
            return null;
        }
    }
    
    return null;
}

        // FIX: Missing logic finished completely, and auto-syncs the chosen file path straight to applicants table column 
        private void SaveDocPath(string type, string path)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "INSERT INTO ApplicantDocuments (ApplicantID, DocumentType, FilePath, DocStatus) VALUES (@Aid, @Type, @Path, 'Submitted') ON DUPLICATE KEY UPDATE FilePath=@Path";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Aid", _loggedInApplicantId);
                        cmd.Parameters.AddWithValue("@Type", type);
                        cmd.Parameters.AddWithValue("@Path", path);
                        cmd.ExecuteNonQuery();
                    }

                    // MASTER FIX: Synchronize the path directly to the applicant profile table row automatically!
                    if (type.Equals("Resume", StringComparison.OrdinalIgnoreCase))
                    {
                        string syncQuery = $"UPDATE {DatabaseHelper.ApplicantTable} SET ResumeFilePath = @Path WHERE ApplicantID = @Aid";
                        using (var syncCmd = new MySqlCommand(syncQuery, conn))
                        {
                            syncCmd.Parameters.AddWithValue("@Path", path);
                            syncCmd.Parameters.AddWithValue("@Aid", _loggedInApplicantId);
                            syncCmd.ExecuteNonQuery();
                        }
                    }

                    ShowMessage("Document Uploaded", $"{type} path synchronized to profile registry entry.");
                }
            }
            catch (Exception ex) { ShowMessage("Upload Logging Error", ex.Message); }
        }

        private void LaunchViewer_Click(object sender, RoutedEventArgs e)
{
    // The ?? "" ensures that if Text is null, it passes an empty string instead
    OpenDocument(txtResumePath.Text ?? "");
}

        private void OpenDocument(string filePath)
{
    try
    {
        if (string.IsNullOrWhiteSpace(filePath) || !System.IO.File.Exists(filePath))
        {
            ShowMessage("File Error", "The file path is empty or the file no longer exists on this device.");
            return;
        }

        var processInfo = new System.Diagnostics.ProcessStartInfo
        {
            FileName = filePath,
            UseShellExecute = true // This is required for macOS to open the default viewer (Preview/Word/Acrobat)
        };
        
        System.Diagnostics.Process.Start(processInfo);
    }
    catch (Exception ex)
    {
        ShowMessage("Launch Error", $"Failed to open file: {ex.Message}");
    }
}

        private void LoadSavedDocumentPaths()
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT DocumentType, FilePath FROM ApplicantDocuments WHERE ApplicantID = @Aid";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Aid", _loggedInApplicantId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string docType = reader["DocumentType"].ToString() ?? "";
                                string path = reader["FilePath"].ToString() ?? "";

                                if (docType.Equals("Resume", StringComparison.OrdinalIgnoreCase)) txtResumePath.Text = path;
                                else if (docType.Equals("ID", StringComparison.OrdinalIgnoreCase)) txtIdPath.Text = path;
                                else if (docType.Equals("Transcript", StringComparison.OrdinalIgnoreCase)) txtTranscriptPath.Text = path;
                                else if (docType.Equals("Certificate", StringComparison.OrdinalIgnoreCase)) txtCertificatePath.Text = path;
                            }
                        }
                    }
                }
            }
            catch { }
        }

        private void LoadClosedJobs()
        {
            _closedJobsList.Items.Clear();
            var jobRepo = new JobRepository();
            var allJobs = jobRepo.GetAllJobs();

            foreach (var job in allJobs)
            {
                if (job.Status.Equals("Closed", StringComparison.OrdinalIgnoreCase))
                {
                    _closedJobsList.Items.Add($"Job #{job.VacancyID}: {job.JobTitle} ({job.Department}) — [FILLED/CLOSED]");
                }
            }
        }

        private bool IsApplicationLocked()
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = $"SELECT COUNT(*) FROM {DatabaseHelper.ApplicationTable} WHERE ApplicantID=@Aid AND Status NOT IN ('Draft', 'Rejected')";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Aid", _loggedInApplicantId);
                        return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                    }
                }
            }
            catch { return false; }
        }

        private void ApplyLockState()
        {
            if (IsApplicationLocked())
            {
                lblDocStatus.Text = "Status Check: Profile Locked (HR Evaluation Underway)";
                lblDocStatus.Foreground = Brushes.OrangeRed;
            }
            else
            {
                lblDocStatus.Text = "Status Check: Unlocked (Editing Allowed)";
                lblDocStatus.Foreground = Brushes.LightGreen;
            }
        }

        private void ShowMessage(string title, string message)
        {
            System.Diagnostics.Debug.WriteLine($"[{title.ToUpper()}] {message}");
        }
    }
}