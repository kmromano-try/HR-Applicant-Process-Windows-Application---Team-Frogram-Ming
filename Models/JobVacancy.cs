using System;

namespace HR_Applicant_System.Models
{
    public class JobVacancy
    {
        // Aligned with database primary key 'VacancyID'
        public int VacancyID { get; set; }
        
        public string JobTitle { get; set; } = string.Empty;
        
        public string Department { get; set; } = string.Empty;
        
        public string JobDescription { get; set; } = string.Empty;
        
        // Added to match the NOT NULL text column in your MySQL table
        public string Qualifications { get; set; } = string.Empty; 
        
        // Aligned with database column 'Status' (Defaults to "Active")
        public string Status { get; set; } = "Active"; 
    }
}