using BugBoard.Api.Models.Account;

namespace BugBoard.Api.Services.Notifications
{
    public interface INotificationRecipientService
    {
        Task<List<ApplicationUser>> GetStatusChangeRecipientsAsync(int bugReportId);
        Task<List<ApplicationUser>> GetCommentRecipientsAsync(int bugReportId, bool isInternal);
    }
}
