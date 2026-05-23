namespace BugBoard.Api.ViewModels.Admin
{
    public class EditUserRolesViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;

        public string SelectedRole { get; set; } = string.Empty;
        public List<string> AvailableRoles { get; set; } = new();
    }
}
