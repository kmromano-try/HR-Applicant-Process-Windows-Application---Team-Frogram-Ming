using System;

namespace HR_Applicant_System.Models
{
    public class InterviewSchedule
    {
        public int InterviewID { get; set; }
        public int ApplicationID { get; set; }
        public DateTime InterviewDate { get; set; }
        public string Interviewer { get; set; }
        public string ModeOrLocation { get; set; }
        public string Status { get; set; } = "Scheduled";
    }
}