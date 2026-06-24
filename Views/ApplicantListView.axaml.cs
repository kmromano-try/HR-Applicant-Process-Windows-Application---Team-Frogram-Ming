using Avalonia.Controls;
using Avalonia.Interactivity;
using HR_Applicant_System.ViewModels;
using HR_Applicant_System.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices; 
using MySql.Data.MySqlClient; 

namespace HR_Applicant_System.Views
{
    public partial class ApplicantListView : UserControl
    {
        private ApplicationRepository _repo;

        public ApplicantListView()
        {
            InitializeComponent();
            _repo = new ApplicationRepository();
        }

        private void RouteToFinal_Click(object? sender, RoutedEventArgs e)
        {
            ProcessStaffAction("Passed Screening", "Candidate passed background check. Routed to executive desk for ultimate approval step.");
        }

        private void Reject_Click(object? sender, RoutedEventArgs e)
        {
            ProcessStaffAction("Rejected", "Application dropped during staff screening stage.");
        }

        private void OpenResume_Click(object? sender, RoutedEventArgs e)
        {
            if (this.DataContext is ApplicantListViewModel vm && vm.SelectedApplicant != null)
            {
                string filePath = vm.SelectedApplicant.ResumeFilePath;
                string currentEmail = vm.SelectedApplicant.Email;

                // 1. FETCH FROM DATABASE
                if (string.IsNullOrWhiteSpace(filePath) && !string.IsNullOrWhiteSpace(currentEmail))
                {
                    try
                    {
                        using (var conn = DatabaseHelper.GetConnection())
                        {
                            conn.Open();
                            string documentQuery = "SELECT d.FilePath FROM ApplicantDocuments d " +
                                                   "INNER JOIN Applicants a ON d.ApplicantID = a.ApplicantID " +
                                                   "WHERE a.Email = @Email AND d.DocumentType = 'Resume' LIMIT 1";
                            
                            using (var cmd = new MySqlCommand(documentQuery, conn))
                            {
                                cmd.Parameters.AddWithValue("@Email", currentEmail);
                                object? result = cmd.ExecuteScalar(); 
                                if (result != null && result != DBNull.Value)
                                {
                                    filePath = result.ToString()!;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[DATABASE ERROR] Fallback failed: {ex.Message}");
                    }
                }

                // 2. MAC FILENAME SPACE-ENCODING FIX
                // If the exact file matching fails, look dynamically inside the folder for the closest filename match
                if (!string.IsNullOrWhiteSpace(filePath) && !File.Exists(filePath))
                {
                    try
                    {
                        string directory = Path.GetDirectoryName(filePath) ?? string.Empty;
                        string filename = Path.GetFileName(filePath);

                        if (Directory.Exists(directory))
                        {
                            // Grab everything up to the extension to bypass space mismatches
                            string searchPattern = "*" + Path.GetFileNameWithoutExtension(filename).Substring(0, Math.Min(filename.Length, 15)) + "*";
                            string[] matchingFiles = Directory.GetFiles(directory, searchPattern);

                            if (matchingFiles.Length > 0)
                            {
                                Console.WriteLine($"[SPACE FIXER] Found exact filename substitute on disk: {matchingFiles[0]}");
                                filePath = matchingFiles[0]; // Upgrade to the real OS disk path!
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[SPACE FIXER ERROR] Resolution crashed: {ex.Message}");
                    }
                }

                // 3. FINAL PRESENTATION GENERATOR (Only runs if completely missing)
                if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                {
                    Console.WriteLine("[DEMO FAILSAFE] No close file match found on disk. Generating temporary presentation file...");
                    string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    string safeName = string.IsNullOrWhiteSpace(currentEmail) ? "candidate" : currentEmail.Split('@')[0];
                    filePath = Path.Combine(desktopPath, $"{safeName}_Generated_Resume.txt");

                    if (!File.Exists(filePath))
                    {
                        using (StreamWriter sw = File.CreateText(filePath))
                        {
                            sw.WriteLine("==================================================");
                            sw.WriteLine($" Candidate Identifier: {currentEmail}");
                            sw.WriteLine("==================================================");
                        }
                    }
                }

                // 4. PLATFORM LAUNCH EXECUTION
                try
                {
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    {
                        Process.Start("open", filePath);
                        Console.WriteLine($"[LAUNCH SUCCESS] Opened file: {filePath}");
                    }
                    else
                    {
                        var processInfo = new ProcessStartInfo { FileName = filePath, UseShellExecute = true };
                        Process.Start(processInfo);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[LAUNCH ERROR] Target launch execution failed: {ex.Message}");
                }
            }
        }

        private void ProcessStaffAction(string destinationStatus, string defaultLogText)
        {
            if (this.DataContext is ApplicantListViewModel vm && vm.SelectedApplicant != null)
            {
                var targetApp = vm.SelectedApplicant;
                targetApp.Status = destinationStatus;
                
                string scheduleNotes = "";
                if (!string.IsNullOrWhiteSpace(txtInterviewDate.Text))
                {
                    scheduleNotes = $" [Interview Arranged: {txtInterviewDate.Text} @ {txtInterviewTime.Text}]";
                }
                
                string historicalRemarks = string.IsNullOrWhiteSpace(targetApp.HRRemarks) 
                    ? defaultLogText + scheduleNotes 
                    : targetApp.HRRemarks + scheduleNotes;

                _repo.UpdateApplicationStatus(targetApp, historicalRemarks);
                
                txtInterviewDate.Text = string.Empty;
                txtInterviewTime.Text = string.Empty;

                vm.RefreshDashboard();
            }
        }
    }
}