using BugBoard.Api.Models.BugReports;

namespace BugBoard.Api.ViewModels.Api
{
    public class BugReportAgentViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public BugStatus Status { get; set; }
        public BugPriority Priority { get; set; }
        public string CreatedByName { get; set; } = string.Empty;
        public string SupervisorName {  get; set; } = string.Empty;
        public string AssignedToName {  get; set; } = string.Empty;
        public List<string> Comments { get; set; } = new List<string>();
    }
}
