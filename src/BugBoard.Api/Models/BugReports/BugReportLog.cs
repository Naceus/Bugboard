namespace BugBoard.Api.Models.BugReports
{
    public class BugReportLog
    {
        int Id { get; set; }
        BugReport BugReportId { get; set; }
        string Message { get; set; } = string.Empty;
        DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        string? AssignedTo {  get; set; }
    }
}
