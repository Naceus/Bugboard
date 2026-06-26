namespace BugBoard.Api.ViewModels.BugReports
{
    public class AddAttachmentsToBugReportViewModel
    {
        public int BugReportId { get; set; }
        public List<IFormFile>? Attachments { get; set; }


    }
}
