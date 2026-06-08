namespace BugBoard.Api.ViewModels.Dashboard
{
    public class DashboardRecentReportViewModel
    {
        public int Id { get; set; }
        public string? Title { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public DateTime LastActivityAt { get; set; }

    }
}