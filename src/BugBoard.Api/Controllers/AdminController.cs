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

		/// <summary>
		/// Display an overview of all registered users with their assigned roles.
		/// </summary>
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
					FullName = $"{user.FirstName} {user.LastName}".Trim(),
					Roles = string.Join(", ", roles)
				});
			}
			return View(model);

		}
		/// <summary>
		/// Displays the edit form for a selected user.
		/// </summary>
		/// <param name="id">The id of the user to edit.</param>
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

		/// <summary>
		/// Updates the selected user's profile data and assigned role.
		/// </summary>
		/// <param name="id">The id of the user being edited.</param>
		/// <param name="model">The submitted user edit form data.</param>
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
				var viewModel = await BuildEditUserErrorViewModelAsync(user, model);
				
				return View(viewModel);

			}
			if (!IsValidRole(model.SelectedRole)){
				ModelState.AddModelError(nameof(model.SelectedRole), "Invalid role selected.");

				var viewModel = await BuildEditUserErrorViewModelAsync(user, model);

				return View(viewModel);
			}

			if(IsRemovingOwnAdminRole(user, model.SelectedRole)){

				ModelState.AddModelError(string.Empty, "You cannot remove your own admin role.");

				var viewModel = await BuildEditUserErrorViewModelAsync(user, model);
				return View(viewModel);
			}

			UpdateUserData(user, model);

			var updateResult = await _userManager.UpdateAsync(user);

			if (!updateResult.Succeeded)
			{
				AddErrors(updateResult);

                var viewModel = await BuildEditUserErrorViewModelAsync(user, model);
                return View(viewModel);
			}
			var roleResult = await UpdateUserRoleAsync(user, model.SelectedRole);

			if (!roleResult.Succeeded)
			{
				AddErrors(roleResult);

                var viewModel = await BuildEditUserErrorViewModelAsync(user, model);
                return View(viewModel);
			}
			return RedirectToAction(nameof(Index));

		}

		/// <summary>
		/// Builds the view model used by the user edit page.
		/// Includes user data, the currently selected role and all available roles.
		/// </summary>
		/// <param name="user">The user for whom the edit view model should be created.</param>
		/// <returns>A populated edit user view model.</returns>
		private async Task<EditUserViewModel> BuildEditUserViewModelAsync(ApplicationUser user) {

			var roles = await _userManager.GetRolesAsync(user);

			return new EditUserViewModel
			{
				UserId = user.Id,
				Email = user.Email ?? string.Empty,
				FirstName = user.FirstName,
				LastName = user.LastName,
				SelectedRole = roles.FirstOrDefault() ?? string.Empty,
				AvailableRoles = GetAvailableRoles()
			};
		}

		/// <summary>
		/// Adds Identity errors to the current model state so they can be displayed in the view.
		/// </summary>
		/// <param name="result">The Identity result contraining validation or update errors</param>
		private void AddErrors(IdentityResult result)
		{
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError(string.Empty, error.Description);
			}
		}

		/// <summary>
		/// Copies submitted form values back into a freshly built view model
		/// This keeps user input visible after validation or Identity errors
		/// </summary>
		/// <param name="source">The submitted model containing posted values.</param>
		/// <param name="target">The rebuilt view model used for redisplaying the form.</param>
		private static void CopyPostedValues(EditUserViewModel source, EditUserViewModel target)
		{
			target.FirstName = source.FirstName;
			target.LastName = source.LastName;
			target.Email = source.Email;
			target.SelectedRole = source.SelectedRole;
		}

		/// <summary>
		/// Builds an edit user view model for redisplaying the form after an error.
		/// Preserves the submitted form values while restoring required dropdown data.
		/// </summary>
		/// <param name="user">The user being edited</param>
		/// <param name="postedModel">The Submitted form values.</param>
		/// <returns>A rebuilt edit user view model with preserved posted values.</returns>
		private async Task<EditUserViewModel> BuildEditUserErrorViewModelAsync(ApplicationUser user, EditUserViewModel postedModel)
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

		private static bool IsValidRole(string role)
		{
			return GetAvailableRoles().Contains(role);
		}

		/// <summary>
		/// Checks whether the current admin is trying to remove their own admin role.
		/// </summary>
		/// <param name="user">The user being edited.</param>
		/// <param name="selectedRole">The selected replacement role.</param>
		/// <returns></returns>
		private bool IsRemovingOwnAdminRole(ApplicationUser user, string selectedRole)
		{
			var currentUserId = _userManager.GetUserId(User);
			return user.Id == currentUserId && selectedRole != ApplicationRoles.Admin;
		}

	}
}
	
 