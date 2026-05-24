namespace BugBoard.Api.ViewModels.Admin
{
    public class AdminUserIndexViewModel
    {
        public string? Search { get; set; }
        public string? SelectedRole { get; set; }
        public List<string> AvailableRoles { get; set; } = new();
        public List<AdminUserListItemViewModel> Users { get; set; } = new(); 
    }
}
