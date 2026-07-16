namespace BugBoard.Api.Models.Account
{
    public class ApiKey
    {
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ApplicationUser User { get; set; } = null!;

        public ApiKey()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
