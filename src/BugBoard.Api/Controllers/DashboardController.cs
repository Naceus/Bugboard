using BugBoard.Api.Models.Account;
using BugBoard.Api.Services.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BugBoard.Api.Controllers
{
    [Authorize]
    public class DashboardController : BaseController
    {
        private readonly IDashboardService _dashboardService;
        public DashboardController(IDashboardService dashboardService) {
        
            _dashboardService = dashboardService;
        }
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId)) { 
                return Unauthorized();
            }

            bool isStaffUser = IsStaffUser();
           
            var model = await _dashboardService.GetDashboardAsync(userId, isStaffUser);

            return View(model);
        }
    }
}
