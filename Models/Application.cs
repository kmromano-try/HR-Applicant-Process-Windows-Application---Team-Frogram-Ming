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
    }
}