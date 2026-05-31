using System.ComponentModel.DataAnnotations;

namespace BugBoard.Api.ViewModels.Account
{
    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; } = string.Empty;

        [Required]
        public string Token { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "The passwords do not match.")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

    }
}
