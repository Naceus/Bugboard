using BugBoard.Api.Models.BugReports;

namespace BugBoard.Api.ViewModels.BugReports
{
    public class CreateBugReportViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public BugPriority Priority { get; set; }
        public List<IFormFile>? Attachments { get; set; }
    }
}
