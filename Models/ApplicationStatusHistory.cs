using System;

namespace HR_Applicant_System.Models
{
    public class ApplicationStatusHistory
    {
        public int HistoryID { get; set; }
        public int ApplicationID { get; set; }
        public string OldStatus { get; set; } = string.Empty;
        public string NewStatus { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
        public DateTime ChangeDate { get; set; } = DateTime.Now;
    }
}