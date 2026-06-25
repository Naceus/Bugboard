using BugBoard.Api.Models.BugReports;

namespace BugBoard.Api.ViewModels.BugReports
{
    public class BugReportDetailsViewModel
    {
        public BugReport BugReport { get; set; } = null!;
        public List<BugReportLog> Logs { get; set; } = new();
        public List<BugReportCommentViewModel> Comments { get; set; } = new();
        public List<BugReportActivityItemViewModel> ActivityItems { get; set; } = new();
        public CreateBugReportCommentViewModel NewComment { get; set; } = new();
        public bool CanCreateInternalComment { get; set; }
        public List<BugReportAttachmentViewModel> Attachments { get; set; } = new();
    }
}
