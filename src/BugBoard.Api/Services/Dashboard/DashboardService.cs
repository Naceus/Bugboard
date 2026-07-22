using BugBoard.Api.Data;
using BugBoard.Api.Models.BugReports;
using BugBoard.Api.ViewModels.Dashboard;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace BugBoard.Api.Services.Dashboard
{
    public class DashboardService : IDashboardService
    {
        private readonly BugBoardDbContext _context;
        private readonly IStringLocalizer<SharedResource> _localizer;
        public DashboardService(BugBoardDbContext context, IStringLocalizer<SharedResource> localizer) {
            _context = context;
            _localizer = localizer;
        }
        public async Task<DashboardViewModel> GetDashboardAsync(string userId, bool isStaff)
        {
            var viewModel = new DashboardViewModel();

            if (string.IsNullOrWhiteSpace(userId)) {
                return viewModel;
            }

            var bugReports = _context.BugReports
                .AsNoTracking()
                .Where(b => b.CreatedByUserId == userId || b.AssignedToId == userId);

            int unassignedCount = await _context.BugReports.CountAsync(b => b.AssignedToId == null);
            int openCount = await bugReports.CountAsync(b => b.Status == BugStatus.Open);
            int inProgressCount = await bugReports.CountAsync(b => b.Status == BugStatus.InProgress);
            int closedCount = await bugReports.CountAsync(b => b.Status == BugStatus.Closed);
           

            viewModel.Metrics = BuildMetrics(unassignedCount, openCount, inProgressCount, closedCount, isStaff);

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

        private DashboardMetricViewModel CreateMetric(string title, int value, string description, string icon, string variant, string? filterUrl)
        {
                   DashboardMetricViewModel viewModel = new(title, value.ToString(), description, icon, variant, filterUrl);

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

        private List<DashboardMetricViewModel> BuildMetrics(int unassignedCount,int openCount, int inProgressCount, int closedCount, bool isStaff)
        {

            var unassignedMetric = CreateMetric(_localizer["Unassigned Reports"], unassignedCount, _localizer["Reports waiting to be assigned"], "?", "danger", "/BugReports?unassigned=true");
            var openMetric = CreateMetric(_localizer["Open Reports"], openCount, _localizer["Reports waiting for review"], "!", "warning", "/BugReports?status=Open");
            var inProgressMetric = CreateMetric(_localizer["In Progress"], inProgressCount, _localizer["Reports currently being worked on"], "↻", "primary", "/BugReports?status=InProgress");
            var closedMetric = CreateMetric(_localizer["Closed"], closedCount, _localizer["Reports completed"], "✓", "success", "/BugReports?status=Closed");

          
         
            if (isStaff)
            {
                return new List<DashboardMetricViewModel>
                {
                    unassignedMetric,
                    openMetric,
                    inProgressMetric,
                    closedMetric
                };
            }
            else
            {
                return new List<DashboardMetricViewModel>
                {
                    openMetric,
                    inProgressMetric,
                    closedMetric
                };

            }

        }

    }
}
