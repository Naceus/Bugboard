using BugBoard.Api.Models.Account;

namespace BugBoard.Api.Models.BugReports
{
    public class BugReportLog
    {
        public int Id { get; set; }
        public int BugReportId { get; set; }
        public BugReport BugReport { get; set; } = null!;
        public string? Message { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? AssignedToId {  get; set; }
        public ApplicationUser? AssignedToUser { get; set; }
    }
}
