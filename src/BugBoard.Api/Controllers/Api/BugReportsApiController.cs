using BugBoard.Api.Data;
using BugBoard.Api.Models.BugReports;
using BugBoard.Api.ViewModels.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BugBoard.Api.Controllers.Api
{
    [Route("api/[controller]")]
    public class BugReportsApiController : ControllerBase
    {
        private readonly BugBoardDbContext _context;

        public BugReportsApiController(BugBoardDbContext context)
        {
            _context = context;
        }
     
        [HttpPost]
        public async Task<IActionResult> CreateTicket([FromBody] CreateBugReportApiViewModel model)
        {
            if (model == null)
            {
                return BadRequest("Model cannot be null");
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();
            }

            var bugReport = new BugReport
                {
                    Title = model.Title,
                    Description = model.Description,
                    Priority = model.Priority,
                    CreatedByUserId = userId,
                    Status = BugStatus.Open,
                    CreateAt = DateTime.UtcNow,
                };
            
            _context.BugReports.Add(bugReport);
            await _context.SaveChangesAsync();

            return Ok(new { id = bugReport.Id });
            
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetTicket(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();
            }

            var bugReport = await _context.BugReports
                            .Include(x => x.CreatedByUser)
                            .Include(x => x.Supervisor)
                            .Include(x => x.AssignedToUser)
                            .FirstOrDefaultAsync(x => x.Id == id);

            if (bugReport == null)
            {
                return NotFound();
            }

            if (bugReport.CreatedByUserId == userId || userId == bugReport.SupervisorId || userId == bugReport.AssignedToId)
            {
                var viewModel = await GetViewModelAsync(bugReport);
                
                return Ok(viewModel);
            }

            return Forbid();
        
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchByTitle([FromQuery] string title)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();
            }

            var bugReports = await _context.BugReports
                            .Include(x => x.CreatedByUser)
                            .Include(x => x.Supervisor)
                            .Include(x => x.AssignedToUser)
                            .Where(x => x.Title == title)
                            .Where( x => x.CreatedByUserId == userId ||
                            x.AssignedToId ==  userId ||
                            x.SupervisorId ==  userId)
                            .ToListAsync();

            if (!bugReports.Any())
            {
                return NotFound();
            }

            var viewModel = new List<BugReportAgentViewModel>();
            foreach(var bugReport in bugReports)
            {
                viewModel.Add(await GetViewModelAsync(bugReport));
            }

            return Ok(viewModel);

            
        }

        private async Task<BugReportAgentViewModel> GetViewModelAsync(BugReport bugReport)
        {
            var comments = await _context.BugReportComments
                           .Where(c => c.BugReportId == bugReport.Id)
                           .Select(c => c.Comment)
                           .ToListAsync();

            BugReportAgentViewModel viewModel = new BugReportAgentViewModel()
            {
                Title = bugReport.Title,
                Description = bugReport.Description,
                Status = bugReport.Status,
                Priority = bugReport.Priority,
                CreatedByName = bugReport.CreatedByUser?.FullName ?? string.Empty,
                SupervisorName = bugReport.Supervisor?.FullName ?? string.Empty,
                AssignedToName = bugReport.AssignedToUser?.FullName ?? string.Empty,
                Comments = comments

            };

            return viewModel;
        }
    }
}
