using BugBoard.Api.Models.BugReports;
using System.ComponentModel.DataAnnotations;

namespace BugBoard.Api.ViewModels.BugReports
{
    public class CreateBugReportCommentViewModel
    {
        public int BugReportId { get; set; }

        [Required]
        [StringLength(2000, ErrorMessage = "Comment cannot be longer than 2000 characters.")]
        public string Comment {  get; set; } = string.Empty;
        public CommentVisibility CommentVisibility { get; set; } = CommentVisibility.Public;

    }
}
