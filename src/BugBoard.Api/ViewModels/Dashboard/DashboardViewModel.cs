namespace BugBoard.Api.ViewModels.Dashboard
{
    public class DashboardViewModel
    {
        public IReadOnlyList<DashboardMetricViewModel> Metrics { get; set; } = new List<DashboardMetricViewModel>();
        public IReadOnlyList<DashboardRecentReportViewModel> RecentReports { get; set; } = new List<DashboardRecentReportViewModel>();
    }
}
