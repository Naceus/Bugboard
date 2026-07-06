using BugBoard.Api.Models.Notifications;
using System.Text.Json;

namespace BugBoard.Api.Services.Notifications
{
    public class NotificationService : INotificationService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        public NotificationService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task SendNotificationAsync(BugReportNotificationPayload payload)
        {
            var jsonstring = JsonSerializer.Serialize(payload, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase});
            StringContent content = new StringContent(jsonstring, System.Text.Encoding.UTF8, "application/json");
            var config = _configuration["Notifications:WebhookUrl"];

            if (config != null)
            {
                await _httpClient.PostAsync(config, content);
            }

        }
    }
}
