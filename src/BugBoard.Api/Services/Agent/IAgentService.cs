namespace BugBoard.Api.Services.Agent
{
    public interface IAgentService
    {
        Task<string> SendMessageAsync(string chatInput, string sessionId, string apiKey);
    }
}
