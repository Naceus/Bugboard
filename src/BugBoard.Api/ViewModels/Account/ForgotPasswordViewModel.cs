using System.ComponentModel.DataAnnotations;

namespace BugBoard.Api.ViewModels.Account
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; } = string.Empty;
    }
}
