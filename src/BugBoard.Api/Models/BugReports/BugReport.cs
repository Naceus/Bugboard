using BugBoard.Api.Models.Account;
using System.ComponentModel.DataAnnotations;
namespace BugBoard.Api.Models.BugReports;

public class BugReport
{
    public int Id { get; set; }

    [Required]
    public string? Title { get; set; }

    [Required]
    public string? Description { get; set; }

    public BugStatus Status { get; set; } = BugStatus.Open;
    public BugPriority Priority { get; set; } = BugPriority.Low;
    public string? AssignedTo { get; set; }
    public DateTime CreateAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedByUserId { get; set; }
    public ApplicationUser? CreatedByUser { get; set; }

    public List<BugReportLog> Logs { get; set; } = new();
}

