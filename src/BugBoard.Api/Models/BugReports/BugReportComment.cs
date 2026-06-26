using BugBoard.Api.Models.Account;
using System.ComponentModel.DataAnnotations;

namespace BugBoard.Api.Models.BugReports
{
    public class BugReportComment
    {
        public int Id { get; set; }
        [Required]
        public required string Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedByUserId{  get; set; }
        public ApplicationUser? CreatedByUser{ get; set; }
        public required string CreatedByName { get; set; }
        public int BugReportId { get; set; }
        public BugReport BugReport { get; set; } = null!;
        public CommentVisibility CommentVisibility { get; set; }


    }
}
