using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using HR_Applicant_System.Models;
using MySql.Data.MySqlClient;

namespace HR_Applicant_System.Views
{
    public partial class InterviewScheduleModal : Window
    {
        private readonly int _applicationId;

        public InterviewScheduleModal(int appId)
        {
            _applicationId = appId;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void OnSaveSchedule(object sender, RoutedEventArgs e)
        {
            var datePicker = this.FindControl<DatePicker>("IntDate");
            var timeBox = this.FindControl<TextBox>("IntTime");
            var interviewerBox = this.FindControl<TextBox>("IntInterviewer");
            var modeBox = this.FindControl<TextBox>("IntMode");

            if (datePicker == null || timeBox == null || interviewerBox == null || modeBox == null)
                return;

            var date = datePicker.SelectedDate;
            var time = timeBox.Text ?? "";
            var interviewer = interviewerBox.Text ?? "";
            var mode = modeBox.Text ?? "";

            if (!date.HasValue)
                return;

            if (date.Value.Date < DateTime.Now.Date)
                return;

            string query = @"
                INSERT INTO InterviewSchedules
                (ApplicationID, InterviewDate, InterviewTime, InterviewerName, Mode, Status)
                VALUES
                (@id, @date, @time, @interviewer, @mode, 'Scheduled');";

            using var conn = DatabaseHelper.GetConnection();
            using var command = new MySqlCommand(query, conn);

            command.Parameters.AddWithValue("@id", _applicationId);
            command.Parameters.AddWithValue("@date", date.Value.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@time", time);
            command.Parameters.AddWithValue("@interviewer", interviewer);
            command.Parameters.AddWithValue("@mode", mode);

            conn.Open();
            command.ExecuteNonQuery();

            Close();
        }
    }
}