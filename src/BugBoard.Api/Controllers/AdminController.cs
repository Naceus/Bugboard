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

			foreach (var user in users){
				var roles = await _userManager.GetRolesAsync(user);
				model.Add(new AdminUserListItemViewModel
				{
					Id = user.Id,
					Email = user.Email ?? string.Empty,
					FullName = $"{user.FirstName} {user.LastName}",
					Roles = string.Join(", ", roles)
				});
			}
			return View(model);

		}

		[HttpGet]
		public async Task<IActionResult> EditRoles(string? id)
		{
			if (string.IsNullOrWhiteSpace(id)){
				return NotFound();
			}

			var user = await _userManager.FindByIdAsync(id);
			if (user == null){
				return NotFound();
			}

			var model = await BuildEditUserRolesViewModelAsync(user);

			return View(model);

		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditRoles(string id, EditUserRolesViewModel model)
		{
			if (id != model.UserId){
				return NotFound();

			}

			var user = await _userManager.FindByIdAsync(id);
			if (user == null){
				return NotFound();
			}

			if (!ModelState.IsValid){
				var viewModel = await BuildEditUserRolesViewModelAsync(user);
				viewModel.SelectedRole = model.SelectedRole;

				return View(viewModel);

			}
			if (!isValidRole(model.SelectedRole)){
				ModelState.AddModelError(nameof(model.SelectedRole), "Invalid role selected.");

				var viewModel = await BuildEditUserRolesViewModelAsync(user);
				viewModel.SelectedRole = model.SelectedRole;

				return View(viewModel);
			}

			var currentRoles = await _userManager.GetRolesAsync(user);
			if(currentRoles.Any()){

				var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
				if (!removeResult.Succeeded){
					AddErrors(removeResult);

					var viewModel = await BuildEditUserRolesViewModelAsync(user);
					viewModel.SelectedRole = model.SelectedRole;
					return View(viewModel);
				}
			}
			;
			
			var addResult = await _userManager.AddToRoleAsync(user, model.SelectedRole);
			if (!addResult.Succeeded)
			{
				AddErrors(addResult);

				var viewModel = await BuildEditUserRolesViewModelAsync(user);
				viewModel.SelectedRole = model.SelectedRole;
				return View(viewModel);
			}
			return RedirectToAction(nameof(Index));

		}
		private async Task<EditUserRolesViewModel> BuildEditUserRolesViewModelAsync(ApplicationUser user) {

			var roles = await _userManager.GetRolesAsync(user);

			return new EditUserRolesViewModel
			{
				UserId = user.Id,
				Email = user.Email ?? string.Empty,
				FullName = $"{user.FirstName} {user.LastName}",
				SelectedRole = roles.FirstOrDefault() ?? string.Empty,
				AvailableRoles = new List<string>
				{
					ApplicationRoles.Admin,
					ApplicationRoles.Developer,
					ApplicationRoles.Reporter
				}
			};
		}

		private void AddErrors(IdentityResult result)
		{
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError(string.Empty, error.Description);
			}
		}

		private static List<string> GetAvailableRoles()
		{
			return new List<string>
			{
				ApplicationRoles.Admin,
				ApplicationRoles.Developer,
				ApplicationRoles.Reporter
			};
		}

		private static bool isValidRole(string role)
		{
			return GetAvailableRoles().Contains(role);
		}

	}
}
	
 