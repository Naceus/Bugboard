namespace BugBoard.Api.Models.BugReports
{
    public record BugReportChange(
        string FieldName,
        string? OldValue,
        string? NewValue
    );
}
