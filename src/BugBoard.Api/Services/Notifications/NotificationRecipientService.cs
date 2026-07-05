using BugBoard.Api.Data;
using BugBoard.Api.Models.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BugBoard.Api.Services.Notifications
{
    public class NotificationRecipientService : INotificationRecipientService
    {
        private readonly BugBoardDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public NotificationRecipientService(BugBoardDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<List<ApplicationUser>> GetStatusChangeRecipientsAsync(int bugReportId)
        {
            var bugReport = await _context.BugReports
                                .Include(b => b.CreatedByUser)
                                .Include(b => b.Supervisor)
                                .FirstOrDefaultAsync(b => b.Id == bugReportId);

            var subscription = await _context.BugReportSubscriptions
                                .Where(s => s.BugReportId == bugReportId && s.NotifyOnStatusChange == true)
                                .Include(s => s.User)
                                .ToListAsync();

            var recipients = new List<ApplicationUser>();

            if (bugReport?.CreatedByUser != null)
            {
                recipients.Add(bugReport.CreatedByUser);
            }
            if (bugReport?.Supervisor != null)
            {
                recipients.Add(bugReport.Supervisor);
            }

            foreach (var s in subscription)
            {
                if (s.User != null)
                {
                    recipients.Add(s.User);

                }
            }
            return recipients.DistinctBy(r => r.Id).ToList();

        }

        public async Task<List<ApplicationUser>> GetCommentRecipientsAsync(int bugReportId, bool isInternal)
        {
            var subscription = await _context.BugReportSubscriptions
                                        .Where(s => s.BugReportId == bugReportId && s.NotifyOnComment == true)
                                        .Include(s => s.User)
                                        .ToListAsync();

            var recipients = new List<ApplicationUser>();

            if (isInternal)
            {
                var admins = await _userManager.GetUsersInRoleAsync("Admin");
                var developers = await _userManager.GetUsersInRoleAsync("Developer");
                var staffIds = admins.Concat(developers).Select(u => u.Id).ToHashSet();

                foreach (var user in subscription)
                {
                    if (user != null)
                    {
                        if (user.User != null && staffIds.Contains(user.User.Id))
                        {
                            recipients.Add(user.User);
                        }
                    }
                }
            }
            else
            {
                foreach (var user in subscription)
                {
                    if (user.User != null)
                    {
                        recipients.Add(user.User);
                    }
                }
            }
            return recipients;

        }
    }
}
    
        
    

