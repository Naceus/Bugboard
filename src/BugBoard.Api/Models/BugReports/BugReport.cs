using BugBoard.Api.Models.Account;
using System.ComponentModel.DataAnnotations;
namespace BugBoard.Api.Models.BugReports;

public class BugReport
{
    public int Id { get; set; }

    [Required]
    public required string Title { get; set; }

    [Required]
    public required string Description { get; set; }

    public BugStatus Status { get; set; } = BugStatus.Open;
    public BugPriority Priority { get; set; } = BugPriority.Low;
    public string? AssignedTo { get; set; }
    public DateTime CreateAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedByUserId { get; set; }
    public ApplicationUser? CreatedByUser { get; set; }

    public List<BugReportLog> Logs { get; set; } = new();
    public List<BugReportComment> Comments { get; set; } = new();
    public List<BugReportAttachment> Attachments { get; set; } = new();
}

