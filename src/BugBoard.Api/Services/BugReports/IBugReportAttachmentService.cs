using Microsoft.AspNetCore.Http;

namespace BugBoard.Api.Services.BugReports
{
    public interface IBugReportAttachmentService
    {
        Task SaveAttachmentsAsync(int bugReportId, IReadOnlyCollection<IFormFile> files, string? uploadedByUserId);
    }
}
