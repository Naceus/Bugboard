using BugBoard.Api.Models.Account;

namespace BugBoard.Api.Models.BugReports
{
    public class BugReportSubscription
    {
        public int Id { get; set; }
        public int BugReportId { get; set; }
        public BugReport BugReport { get; set; } = null!;
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
        public bool NotifyOnStatusChange { get; set; }
        public bool NotifyOnComment {  get; set; }
        
    }
}
