using System.Text.Json;

namespace BugBoard.Api.Services.Agent
{
    public class AgentService : IAgentService
    {

        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public AgentService (HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<string> SendMessageAsync(string chatInput, string sessionId, string apiKey)
        {
            var jsonstring = JsonSerializer.Serialize(new { action = "sendMessage", chatInput, sessionId, apiKey},
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
            StringContent content = new StringContent(jsonstring, System.Text.Encoding.UTF8, "application/json");

            var config = _configuration["Agent:WebhookUrl"];

            if (config == null)
            {
                throw new InvalidOperationException("Agent:WebhookUrl is not configured.");
            }
            
            var response = await _httpClient.PostAsync(config, content);

            return await response.Content.ReadAsStringAsync();
        
        }
           
    }
}
