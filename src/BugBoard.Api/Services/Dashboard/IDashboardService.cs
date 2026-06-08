
using BugBoard.Api.ViewModels.Dashboard;

namespace BugBoard.Api.Services.Dashboard
{
    public interface IDashboardService 
    {
        public Task<DashboardViewModel> GetReporterDashboardAsync(string userId);
     
    }
}
