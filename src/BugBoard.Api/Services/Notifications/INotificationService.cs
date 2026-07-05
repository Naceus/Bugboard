using BugBoard.Api.Models.Notifications;

namespace BugBoard.Api.Services.Notifications
{
    public interface INotificationService
    {
        Task SendNotificationAsync(BugReportNotificationPayload payload);
    }
}
