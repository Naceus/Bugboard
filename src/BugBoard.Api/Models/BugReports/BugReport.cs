using System.ComponentModel.DataAnnotations;
namespace BugBoard.Api.Models.BugReports;

public class BugReport
{
    public int Id { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    public BugStatus Status { get; set; } = BugStatus.Open;
    public BugPriority Priority { get; set; } = BugPriority.Medium;
    public string? AssignedTo { get; set; }
    public DateTime CreateAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

