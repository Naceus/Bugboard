using BugBoard.Api.Models.Account;
using BugBoard.Api.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BugBoard.Api.Controllers
{
    [Authorize(Roles = ApplicationRoles.Admin)]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
     
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var model = new List<AdminUserListItemViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                model.Add(new AdminUserListItemViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = $"{user.FirstName} {user.LastName}",
                    Roles = string.Join(",", roles)
                });
                
            }
            return View(model);
        }
    }
}
