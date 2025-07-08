namespace HelpdeskBlazor.Models
{
    public class TicketReportItem
    {
        public string RequestNo { get; set; } = "";
        public string AppRefNo { get; set; } = "";
        public DateTime RequestedDate { get; set; }
        public string RequestedBy { get; set; } = "";
        public string EndorsedTo { get; set; } = "";
        public string TicketNo { get; set; } = "";
        public string RequestType { get; set; } = "";
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public int ResponseDays { get; set; }
        public string Concern { get; set; } = "";
        public string MajorConcern { get; set; } = "";
    }
}