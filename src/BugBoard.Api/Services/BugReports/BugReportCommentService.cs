using BugBoard.Api.Data;
using BugBoard.Api.Models.Account;
using BugBoard.Api.Models.BugReports;
using BugBoard.Api.ViewModels.BugReports;
using Microsoft.EntityFrameworkCore;

namespace BugBoard.Api.Services.BugReports
{
    public class BugReportCommentService : IBugReportCommentService
    {
        private readonly BugBoardDbContext _context;

        public BugReportCommentService(BugBoardDbContext context)
        {
            _context = context;
        }

        public async Task<List<BugReportCommentViewModel>> GetVisibleCommentsAsync(int bugReportId, bool canUseInternalComments)
        {
            IQueryable<BugReportComment> commentsQuery = _context.BugReportComments
                .Where(c => c.BugReportId == bugReportId);

            if (!canUseInternalComments)
            {
                commentsQuery = commentsQuery
                    .Where(c => c.CommentVisibility == CommentVisibility.Public);
            }

            return await commentsQuery
                .OrderBy(c => c.CreatedAt)
                .Select(c => new BugReportCommentViewModel
                {
                    Comment = c.Comment,
                    CreatedByName = c.CreatedByName,
                    CreatedAt = c.CreatedAt,
                    CommentVisibility = c.CommentVisibility
                })
                .ToListAsync();
        }

        public List<BugReportActivityItemViewModel> BuildActivityItems(IEnumerable<BugReportCommentViewModel> comments, IEnumerable<BugReportLog> logs)
        {
            return comments
                .Select(comment => new BugReportActivityItemViewModel
                {
                    Message = comment.Comment,
                    AuthorName = comment.CreatedByName,
                    CreatedAt = comment.CreatedAt,
                    IsComment = true,
                    CommentVisibility = comment.CommentVisibility
                })
                .Concat(logs.Select(log => new BugReportActivityItemViewModel
                {
                    Message = log.Message ?? string.Empty,
                    CreatedAt = log.CreatedAt,
                    IsComment = false
                }))
                .OrderByDescending(item => item.CreatedAt)
                .ToList();
        }

        public async Task CreateCommentAsync(CreateBugReportCommentViewModel commentViewModel, ApplicationUser? currentUser, string? fallbackUserName, bool canUseInternalComments)
        {
            var createdByName = currentUser == null
                ? fallbackUserName ?? "Unknown"
                : $"{currentUser.FirstName} {currentUser.LastName}".Trim();

            if (string.IsNullOrWhiteSpace(createdByName))
            {
                createdByName = currentUser?.UserName ?? "Unknown";
            }

            var comment = new BugReportComment
            {
                BugReportId = commentViewModel.BugReportId,
                Comment = commentViewModel.Comment.Trim(),
                CommentVisibility = canUseInternalComments ? commentViewModel.CommentVisibility : CommentVisibility.Public,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = currentUser?.Id,
                CreatedByName = createdByName
            };

            _context.BugReportComments.Add(comment);
            await _context.SaveChangesAsync();
        }
    }
}
