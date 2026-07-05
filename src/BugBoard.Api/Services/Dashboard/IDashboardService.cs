
using BugBoard.Api.ViewModels.Dashboard;

namespace BugBoard.Api.Services.Dashboard
{
    public interface IDashboardService 
    {
        public Task<DashboardViewModel> GetDashboardAsync(string userId, bool isStaff);
     
    }
}
