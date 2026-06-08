using BugBoard.Api.Models.Account;
using Microsoft.AspNetCore.Identity;

namespace BugBoard.Api.Data.Seed
{
    public class DatabaseSeeder
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        public DatabaseSeeder(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _configuration = configuration;
        }


        /// <summary>
        /// Seeds default identity data required by the application.
        /// Creates the default roles and optionally creates an initial admin user
        /// if Admin credentials are configured.
        /// </summary>
        public async Task SeedAsync()
        {
            string[] roles = { ApplicationRoles.Admin, ApplicationRoles.Developer, ApplicationRoles.Reporter };

            foreach (string role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    var roleResult = await _roleManager.CreateAsync(new IdentityRole(role));
                    ThrowIfFailed(roleResult, $"Failed to seed role '{role}'");
                }
            }
            var adminEmail = _configuration["SeedAdmin:Email"];
            var adminPassword = _configuration["SeedAdmin:Password"];

            if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword)){
                return;
            }

            var adminUser = await _userManager.FindByEmailAsync(adminEmail);
            if ( adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    FirstName = "System",
                    LastName = "Admin",
                    Email = adminEmail,
                    UserName = adminEmail,

                };
                
                var result = await _userManager.CreateAsync(adminUser, adminPassword);
                ThrowIfFailed(result, "Failed to seed admin user");
                
            }

            var isAdmin = await _userManager.IsInRoleAsync(adminUser, ApplicationRoles.Admin);
            if (!isAdmin)
            {
                var roleResult = await _userManager.AddToRoleAsync(adminUser, ApplicationRoles.Admin);
                ThrowIfFailed(roleResult, "Failed to assign admin role");
                
            }                     
             
        }

        /// <summary>
        /// Throws an exception if an Identity operation failed.
        /// Includes all Identity error descriptions in the exception message.
        /// </summary>
        private static void ThrowIfFailed(IdentityResult result, string message)
        {
            if (result.Succeeded)
            {
                return;
            }

            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"{message}: {errors}");
        }
    }
}
