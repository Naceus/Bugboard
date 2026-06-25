namespace BugBoard.Api.ViewModels.BugReports
{
    public class BugReportAttachmentViewModel
    {
        public int Id { get; set; }
        public string OriginalFileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public DateTime UploadedAt {  get; set; } = DateTime.UtcNow;
    }
}
