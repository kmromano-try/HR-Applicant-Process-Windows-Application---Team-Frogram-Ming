using System;

namespace HR_Applicant_System.Models
{
    public class JobVacancy
    {
        public int JobID { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string JobDescription { get; set; } = string.Empty;
        public string VacancyStatus { get; set; } = "Active"; // Active, Closed, Cancelled
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}