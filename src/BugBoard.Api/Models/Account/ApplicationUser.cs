using Microsoft.AspNetCore.Identity;

namespace BugBoard.Api.Models.Account
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

    }
}
