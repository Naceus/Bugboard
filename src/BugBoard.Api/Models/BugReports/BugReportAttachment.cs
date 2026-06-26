using BugBoard.Api.Models.Account;

namespace BugBoard.Api.Models.BugReports
{
    public class BugReportAttachment
    {
        public int Id { get; set;  }
        public int BugReportId { get; set; }
        public BugReport BugReport { get; set; } = null!;
        public string OriginalFileName { get; set; } = string.Empty;
        public string StoredFileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
        public string? UploadedByUserId { get; set; }
        public ApplicationUser? UploadedByUser { get; set; }
    }
}
