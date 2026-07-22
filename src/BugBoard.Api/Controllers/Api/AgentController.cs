using BugBoard.Api.Data;
using BugBoard.Api.Services.Agent;
using BugBoard.Api.ViewModels.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;

namespace BugBoard.Api.Controllers.Api
{
    [Route("api/[controller]")]
    public class AgentController : ControllerBase
    {
        private readonly BugBoardDbContext _context;
        private readonly IAgentService _agentService;

        public AgentController(BugBoardDbContext context, IAgentService agentService)
        {
            _context = context;
            _agentService = agentService;
        }

        [HttpPost]
        public async Task<IActionResult> SendMessageAsync([FromBody]AgentMessageViewModel model)
        {
            if (model == null)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var apiKey = await _context.ApiKeys.FirstOrDefaultAsync(a => a.UserId == userId);
            if (apiKey == null)
            {
                return NotFound();
            }

            var result = await _agentService.SendMessageAsync(model.ChatInput, model.SessionId, apiKey.Key);
            if (result != null)
            {
                return Content(result, "application/json");
            }

            return NotFound();

        }

    }
}
