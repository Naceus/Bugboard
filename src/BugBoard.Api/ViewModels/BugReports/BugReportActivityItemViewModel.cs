using BugBoard.Api.Models.BugReports;

namespace BugBoard.Api.ViewModels.BugReports
{
    public class BugReportActivityItemViewModel
    {
        public string Message { get; set; } = string.Empty;
        public string? AuthorName { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsComment { get; set; }
        public CommentVisibility? CommentVisibility { get; set; }
    }
}
