using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using HR_Applicant_System.Models;
using MySql.Data.MySqlClient;

namespace HR_Applicant_System.Views
{
    public partial class InterviewEvaluationView : Window
    {
        private readonly int _applicationId;

        public InterviewEvaluationView(int appId)
        {
            _applicationId = appId;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void OnSubmitEval(object sender, RoutedEventArgs e)
        {
            var scoreControl = this.FindControl<NumericUpDown>("ScoreInput");
            var remarksControl = this.FindControl<TextBox>("RemarksInput");
            var comboControl = this.FindControl<ComboBox>("PassFailCombo");

            if (scoreControl == null || remarksControl == null || comboControl == null)
                return;

            var score = scoreControl.Value ?? 0;
            var remarks = remarksControl.Text ?? "";

            // If PassFailCombo contains strings
            var recommendation = comboControl.SelectedItem?.ToString() ?? "";

            // If it contains ComboBoxItem instead, use:
            // var recommendation = (comboControl.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";

            string query = @"
                INSERT INTO InterviewEvaluations
                (ApplicationID, Score, Remarks, Recommendation, EvalDate)
                VALUES
                (@id, @score, @remarks, @recommendation, @date);";

            using var conn = DatabaseHelper.GetConnection();
            using var command = new MySqlCommand(query, conn);

            command.Parameters.AddWithValue("@id", _applicationId);
            command.Parameters.AddWithValue("@score", score);
            command.Parameters.AddWithValue("@remarks", remarks);
            command.Parameters.AddWithValue("@recommendation", recommendation);
            command.Parameters.AddWithValue("@date", DateTime.Now.ToString("yyyy-MM-dd"));

            conn.Open();
            command.ExecuteNonQuery();

            Close();
        }
    }
}