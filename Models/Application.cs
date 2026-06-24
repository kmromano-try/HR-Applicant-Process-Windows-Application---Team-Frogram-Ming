using System;

namespace HR_Applicant_System.Models
{
    public class Application
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string HRRemarks { get; set; } = string.Empty;
        
        // Exact matches to your schema columns
        public string Email { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty; 
        public string Bio { get; set; } = string.Empty;           
        public string Experience { get; set; } = string.Empty;    
        public string ResumeFilePath { get; set; } = string.Empty; 
    }
}