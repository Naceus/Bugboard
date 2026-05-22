namespace BugBoard.Api.Models.Account
{
    public static class ApplicationRoles
    {

        public const string Admin = "Admin";
        public const string Developer = "Developer";
        public const string Reporter = "Reporter";

        public const string AdminDeveloper = Admin + "," + Developer;
    }
}
