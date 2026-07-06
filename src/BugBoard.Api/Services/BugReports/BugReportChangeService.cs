using BugBoard.Api.Models.BugReports;

namespace BugBoard.Api.Services.BugReports
{
    public class BugReportChangeService
    {
        public List<BugReportChange> GetChanges(BugReport oldBugReport, BugReport newBugReport)
        {
            var changes = new List<BugReportChange>
            {
                new("Title", oldBugReport.Title, newBugReport.Title),
                new("Description", oldBugReport.Description, newBugReport.Description),
                new("Status", oldBugReport.Status.ToString(), newBugReport.Status.ToString()),
                new("Priority", oldBugReport.Priority.ToString(), newBugReport.Priority.ToString()),
                new("AssignedToId", oldBugReport.AssignedToId, newBugReport.AssignedToId),
            };
            return changes
                .Where(change => change.OldValue != change.NewValue)
                .ToList();

        }
    }
}