using BugBoard.Api.Models.BugReports;
using BugBoard.Api.Services.BugReports;

namespace BugBoard.Api.Tests.Services.BugReports
{
    public class BugReportChangeServiceTests
    {
        [Fact]
        public void GetChanges_NoFieldsChanged_ReturnsEmptyList()
        {
            var oldBugReport = new BugReport
            {
                Title = "Bug A",
                Description = "Description A"
            };

            var newBugReport = new BugReport
            {
                Title = "Bug A",
                Description = "Description A"
            };

            var service = new BugReportChangeService();
            var changes = service.GetChanges(oldBugReport, newBugReport);

            Assert.Empty(changes);
        }

        [Fact]
        public void GetChanges_StatusChanged_ReturnsOneChange()
        {
            var oldBugReport = new BugReport
            {
                Title = "Bug B",
                Description = "Description B",
                Status = BugStatus.Open
            };

            var newBugReport = new BugReport
            {
                Title = "Bug B",
                Description = "Description B",
                Status = BugStatus.InProgress
            };

            var service = new BugReportChangeService();
            var changes = service.GetChanges(oldBugReport, newBugReport);

            var change = Assert.Single(changes);

            Assert.Equal("Status", change.FieldName);
            Assert.Equal(BugStatus.Open.ToString(), change.OldValue);
            Assert.Equal(BugStatus.InProgress.ToString(), change.NewValue);
        }

    }
}
