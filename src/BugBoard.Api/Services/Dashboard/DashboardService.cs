using BugBoard.Api.Data;
using BugBoard.Api.Models.BugReports;
using BugBoard.Api.ViewModels.Dashboard;
using Microsoft.EntityFrameworkCore;

namespace BugBoard.Api.Services.Dashboard
{
    public class DashboardService : IDashboardService
    {
        private readonly BugBoardDbContext _context;
        public DashboardService(BugBoardDbContext context ) {
            _context = context;
        }
        public async Task<DashboardViewModel> GetReporterDashboardAsync(string userId)
        {
            var viewModel = new DashboardViewModel();

            if (string.IsNullOrWhiteSpace(userId)) {
                return viewModel;
            }

            var bugReports = _context.BugReports
                .AsNoTracking()
                .Where(b => b.CreatedByUserId == userId);

            int openCount = await bugReports.CountAsync(b => b.Status == BugStatus.Open);
            int inProgressCount = await bugReports.CountAsync(b => b.Status == BugStatus.InProgress);
            int closedCount = await bugReports.CountAsync(b => b.Status == BugStatus.Closed);



            var openMetric = CreateMetric("Open Reports", openCount, "Reports waiting for review","!","warning");            
            var inProgressMetric = CreateMetric("In Progress", inProgressCount, "Reports currently being worked on", "↻", "primary");
            var closedMetric = CreateMetric("Closed", closedCount, "Reports completed", "✓", "success");

            viewModel.Metrics = new List<DashboardMetricViewModel>
            {
                openMetric,
                inProgressMetric,
                closedMetric
            };

            var recentBugReports = await bugReports
                .OrderByDescending(b => b.UpdatedAt ?? b.CreateAt)
                .Take(5)
                .ToListAsync();

            List<DashboardRecentReportViewModel> recentReportViewModelList = new List<DashboardRecentReportViewModel>();
            foreach (var recentBugReport in recentBugReports)
            {
               
                recentReportViewModelList.Add(MapRecentReport(recentBugReport));
            }

            viewModel.RecentReports = recentReportViewModelList;

            return viewModel;
        }

        private DashboardMetricViewModel CreateMetric(string title, int value, string description, string icon, string variant)
        {
                   DashboardMetricViewModel viewModel = new(title, value.ToString(), description, icon, variant);

            return viewModel;
          
        }

        private DashboardRecentReportViewModel MapRecentReport(BugReport recentBugReport) {

            DashboardRecentReportViewModel recentReportViewModel = new();
            recentReportViewModel.Id = recentBugReport.Id;
            recentReportViewModel.Title = recentBugReport.Title;
            recentReportViewModel.Status = recentBugReport.Status.ToString();
            recentReportViewModel.Priority = recentBugReport.Priority.ToString();
            recentReportViewModel.LastActivityAt = recentBugReport.UpdatedAt ?? recentBugReport.CreateAt;

            return recentReportViewModel;
        }
    }
}
