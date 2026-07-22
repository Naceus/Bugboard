using BugBoard.Api.Models.BugReports;

namespace BugBoard.Api.ViewModels.Api
{
    public class CreateBugReportApiViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public BugPriority Priority {  get; set; } 
    }
}
