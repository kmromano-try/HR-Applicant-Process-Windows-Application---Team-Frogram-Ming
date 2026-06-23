using System;

namespace HR_Applicant_System.Models
{
    public class InterviewSchedule
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }
        public DateTime InterviewDate { get; set; }
        public string Interviewer { get; set; } = string.Empty;
        public string ModeOrLocation { get; set; } = string.Empty;
    }
}