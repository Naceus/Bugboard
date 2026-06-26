using BugBoard.Api.Models.Account;
using BugBoard.Api.Models.BugReports;
using BugBoard.Api.ViewModels.BugReports;

namespace BugBoard.Api.Services.BugReports
{
    public interface IBugReportCommentService
    {
        Task<List<BugReportCommentViewModel>> GetVisibleCommentsAsync(int bugReportId, bool canUseInternalComments);
        List<BugReportActivityItemViewModel> BuildActivityItems(IEnumerable<BugReportCommentViewModel> comments, IEnumerable<BugReportLog> logs);
        Task CreateCommentAsync(CreateBugReportCommentViewModel commentViewModel, ApplicationUser? currentUser, string? fallbackUserName, bool canUseInternalComments);
    }
}
