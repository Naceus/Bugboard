namespace BugBoard.Api.Models.Notifications
{
    public class BugReportNotificationPayload
    {
        public string RecipientEmail { get; set; } = string.Empty;
        public int BugReportId { get; set; }
        public string BugReportTitle { get; set; } = string.Empty;
        public string EventType { get; set; } = string.Empty;
        public string? OldStatus { get; set; }
        public string? NewStatus { get; set; }
        public string? Comment { get; set; }
        public string Priority { get; set; } = string.Empty;
        public string ReporterName { get; set; } = string.Empty;
    }
}
