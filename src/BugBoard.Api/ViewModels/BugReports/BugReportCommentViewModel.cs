using BugBoard.Api.Models.BugReports;

namespace BugBoard.Api.ViewModels.BugReports
{
    public class BugReportCommentViewModel
    {
        public string Comment { get; set; } = string.Empty;
        public string CreatedByName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public CommentVisibility CommentVisibility { get; set; }


    }
}
