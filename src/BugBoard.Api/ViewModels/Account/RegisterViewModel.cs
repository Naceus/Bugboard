using System.ComponentModel.DataAnnotations;

namespace BugBoard.Api.ViewModels.Account
{
    public class RegisterViewModel
    {
        [Required]
        public string firstName { get; set; } = string.Empty;
        [Required]
        public string lastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Password)]
        public string ConfirmPassword {  get; set; } = string.Empty;

    }
}
