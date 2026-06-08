using System;

namespace HR_Applicant_System.Models
{
    public class Application
    {
        public int ApplicationID { get; set; }
        public int ApplicantID { get; set; }
        public int JobVacancyID { get; set; }
        public string Status { get; set; } = "Draft";
        public DateTime SubmissionDate { get; set; }
        public string HRRemarks { get; set; }
        
        public bool IsLocked => Status != "Draft" && Status != "Submitted"; 
    }
}