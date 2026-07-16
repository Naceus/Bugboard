using BugBoard.Api.Data;
using BugBoard.Api.Models.BugReports;
using BugBoard.Api.ViewModels.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BugBoard.Api.Controllers.Api
{
    [Route("api/[controller]")]
    public class BugReportsApiController : Controller
    {
        private readonly BugBoardDbContext _context;

        public BugReportsApiController(BugBoardDbContext context)
        {
            _context = context;
        }
     
        [HttpPost]
        public async Task<IActionResult> CreateTicket(CreateBugReportApiViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();
            }

            var bugReport = new BugReport
                {
                    Description = model.Description,
                    Title = model.Title,
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
                            .FirstOrDefaultAsync(x => x.Id == id);

            if (bugReport == null)
            {
                return NotFound();
            }

            if (bugReport.CreatedByUserId == userId || userId == bugReport.SupervisorId || userId == bugReport.AssignedToId)
            {
                return Ok(bugReport);
            }

            return Forbid();
        
        }
    }
}
