using BugBoard.Api.Data;
using BugBoard.Api.Models.Account;
using BugBoard.Api.ViewModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using SQLitePCL;
using System.Security.Cryptography;
using System.Text;

namespace BugBoard.Api.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _environment;
        private readonly BugBoardDbContext _context;

        public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IWebHostEnvironment environment, BugBoardDbContext context)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _environment = environment;
            _context = context;
        }
        [HttpGet]
        public IActionResult Login()
        {
            if(User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Home/Index.cshtml", model);
            }
            var result = await _signInManager.PasswordSignInAsync(
                model.EmailAddress,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: false);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View("~/Views/Home/Index.cshtml",model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {   
                var user = new ApplicationUser
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.EmailAddress,
                    UserName = model.EmailAddress
                };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    var apiKey = new ApiKey
                    {
                        UserId = user.Id,
                        Key = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)),
                        User = user
                    };

                    var roleResult = await _userManager.AddToRoleAsync(user, ApplicationRoles.Reporter);
                    if (roleResult.Succeeded)
                    {
                        _context.ApiKeys.Add(apiKey);
                        await _context.SaveChangesAsync();
                        
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return RedirectToAction("Index", "BugReports");

                    }
                    await _userManager.DeleteAsync(user);
                    AddErrors(roleResult);
                }
                else
                {
                    AddErrors(result);
                }
            }
            return View(model);

        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword() { 
        
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid) { 
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.EmailAddress);

            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var tokenBytes = Encoding.UTF8.GetBytes(token);
                var encodedToken = WebEncoders.Base64UrlEncode(tokenBytes);
                var resetLink = Url.Action(
                    "ResetPassword",
                    "Account",
                    new
                    {
                        email = model.EmailAddress,
                        token = encodedToken
                    },
                    Request.Scheme);

                // Temporary development convenience until real email sending is implemented:
                // never expose the reset link outside of a development environment.
                if (_environment.IsDevelopment())
                {
                    TempData["ResetPasswordLink"] = resetLink;
                }
            }



            return RedirectToAction("ForgotPasswordConfirmation");
        }

        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string email, string token)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
            {
                return RedirectToAction("Index", "Home");
            }
            ResetPasswordViewModel model = new();
            model.EmailAddress = email;
            model.Token = token;

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid) {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.EmailAddress);

            if (user == null) {
                return RedirectToAction("ResetPasswordConfirmation");
            }

            string decodedToken;
            try
            {
                var tokenBytes = WebEncoders.Base64UrlDecode(model.Token);
                decodedToken = Encoding.UTF8.GetString(tokenBytes);
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "The reset link is invalid or has expired.");
                return View(model);
            }

            var result = await _userManager.ResetPasswordAsync(user, decodedToken, model.Password);
            if (result.Succeeded) {
                return RedirectToAction("ResetPasswordConfirmation");    
            }

            foreach (var error in result.Errors) {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        private void AddErrors(IdentityResult result)
        {
            foreach(var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
    }
}
