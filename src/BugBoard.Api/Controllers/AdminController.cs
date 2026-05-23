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
		public async Task<IActionResult> EditUser(string? id)
		{
			if (string.IsNullOrWhiteSpace(id)){
				return NotFound();
			}

			var user = await _userManager.FindByIdAsync(id);
			if (user == null){
				return NotFound();
			}

			var model = await BuildEditUserViewModelAsync(user);

			return View(model);

		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditUser(string id, EditUserViewModel model)
		{
			if (id != model.UserId){
				return NotFound();

			}

			var user = await _userManager.FindByIdAsync(id);
			if (user == null){
				return NotFound();
			}

			if (!ModelState.IsValid){
				var viewModel = await BuildEditUserErrorVieModelAsync(user, model);
				
				return View(viewModel);

			}
			if (!isValidRole(model.SelectedRole)){
				ModelState.AddModelError(nameof(model.SelectedRole), "Invalid role selected.");

				var viewModel = await BuildEditUserErrorVieModelAsync(user, model);

				return View(viewModel);
			}

			if(IsRemovinOwnAdminRole(user, model.SelectedRole)){

				ModelState.AddModelError(string.Empty, "You cannot remove your own admin role,");

				var viewModel = await BuildEditUserErrorVieModelAsync(user, model);
				return View(viewModel);
			}

			UpdateUserData(user, model);

			var updateResult = await _userManager.UpdateAsync(user);

			if (!updateResult.Succeeded)
			{
				AddErrors(updateResult);

                var viewModel = await BuildEditUserErrorVieModelAsync(user, model);
                return View(viewModel);
			}
			var roleResult = await UpdateUserRoleAsync(user, model.SelectedRole);

			if (!roleResult.Succeeded)
			{
				AddErrors(roleResult);

                var viewModel = await BuildEditUserErrorVieModelAsync(user, model);
                return View(viewModel);
			}
			return RedirectToAction(nameof(Index));

		}
		private async Task<EditUserViewModel> BuildEditUserViewModelAsync(ApplicationUser user) {

			var roles = await _userManager.GetRolesAsync(user);

			return new EditUserViewModel
			{
				UserId = user.Id,
				Email = user.Email ?? string.Empty,
				FirstName = user.FirstName,
				LastName = user.LastName,
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
		private static void CopyPostedValues(EditUserViewModel source, EditUserViewModel target)
		{
			target.FirstName = source.FirstName;
			target.LastName = source.LastName;
			target.Email = source.Email;
			target.SelectedRole = source.SelectedRole;
		}

		private async Task<EditUserViewModel> BuildEditUserErrorVieModelAsync(ApplicationUser user, EditUserViewModel postedModel)
		{
			var viewModel = await BuildEditUserViewModelAsync(user);
			CopyPostedValues(postedModel, viewModel);

			return viewModel;
		}

		private static void UpdateUserData(ApplicationUser user, EditUserViewModel model)
		{
			user.FirstName = model.FirstName;
			user.LastName = model.LastName;
			user.Email = model.Email;
			user.UserName = model.Email;
		}

		private async Task<IdentityResult> UpdateUserRoleAsync(ApplicationUser user, string selectedRole)
		{
			var currentRoles = await _userManager.GetRolesAsync(user);

			if (currentRoles.Any())
			{
				var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);

				if (!removeResult.Succeeded)
				{
					return removeResult;
				}
			}
			return await _userManager.AddToRoleAsync(user, selectedRole);
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

		private bool IsRemovinOwnAdminRole(ApplicationUser user, string selectedRole)
		{
			var currentUserId = _userManager.GetUserId(User);
			return user.Id == currentUserId && selectedRole != ApplicationRoles.Admin;
		}

	}
}
	
 