using BugBoard.Api.Models.BugReports;

namespace BugBoard.Api.ViewModels.BugReports
{
    public class BugReportIndexViewModel
    {
        public List<BugReport> Reports { get; set; } = new();
        public BugStatus? SelectedStatus { get; set; }
        public BugPriority? SelectedPriority { get; set; }
        public string? Search { get; set;  }
        public PaginationViewModel Pagination { get; set; } = new();
    }
}
