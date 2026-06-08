using System;

namespace HR_Applicant_System.Models
{
    public class ApplicationStatusHistory
    {
        public int HistoryID { get; set; }
        public int ApplicationID { get; set; }
        public string StatusChangedTo { get; set; }
        public DateTime ChangeDate { get; set; }
        public string ChangedBy { get; set; }
    }
}