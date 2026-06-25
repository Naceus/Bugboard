using BugBoard.Api.Data;
using BugBoard.Api.Models.Account;
using BugBoard.Api.Models.BugReports;
using BugBoard.Api.Services.BugReports;
using BugBoard.Api.ViewModels.BugReports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BugBoard.Api.Controllers
{
    [Authorize]
    public class BugReportsController : Controller
    {
        private readonly BugBoardDbContext _context;
        private readonly BugReportChangeService _bugReportChangeService;
        private readonly IBugReportCommentService _bugReportCommentService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IBugReportAttachmentService _bugReportAttachmentService;
        public BugReportsController(
            BugBoardDbContext context,
            BugReportChangeService bugReportChangeService,
            IBugReportCommentService bugReportCommentService,
            IBugReportAttachmentService bugReportAttachmentService,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _bugReportChangeService = bugReportChangeService;
            _bugReportCommentService = bugReportCommentService;
            _bugReportAttachmentService = bugReportAttachmentService;
            _userManager = userManager;
        }

        // GET: BugReports
        public async Task<IActionResult> Index(BugStatus? status, BugPriority? priority, string? title, int page = 1)
        {

            var viewModel = await BuildIndexViewModel(status, priority, title, page);

            return View(viewModel);
        }

        // GET: BugReports/Details/5

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bugReport = await _context.BugReports
                .Include(b => b.Logs.OrderByDescending(l => l.CreatedAt))
                .Include(b => b.CreatedByUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bugReport == null)
            {
                return NotFound();
            }

            if (!CanViewBugReport(bugReport))
            {
                return Forbid();
            }
            var canUseInternalComments = IsStaffUser();

            var comments = await _bugReportCommentService.GetVisibleCommentsAsync(bugReport.Id, canUseInternalComments);
            var activityItems = _bugReportCommentService.BuildActivityItems(comments, bugReport.Logs);
            var viewModel = new BugReportDetailsViewModel
            {
                BugReport = bugReport,
                Logs = bugReport.Logs,
                Comments = comments,
                ActivityItems = activityItems,
                NewComment = new CreateBugReportCommentViewModel
                {
                    BugReportId = bugReport.Id
                },
                CanCreateInternalComment = canUseInternalComments
            };
            
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateComment(CreateBugReportCommentViewModel commentViewModel)
        {
            var bugReport = await _context.BugReports
                .FirstOrDefaultAsync(b => b.Id == commentViewModel.BugReportId);

            if (bugReport == null)
            {
                return NotFound();
            }

            if (!CanViewBugReport(bugReport))
            {
                return Forbid();
            }

            var canUseInternalComments = IsStaffUser();

            commentViewModel.Comment = commentViewModel.Comment.Trim();
            if (string.IsNullOrWhiteSpace(commentViewModel.Comment))
            {
                ModelState.AddModelError(nameof(commentViewModel.Comment), "Comment is required.");
            }

            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Details), new { id = commentViewModel.BugReportId });
            }

            var currentUser = await _userManager.GetUserAsync(User);
            await _bugReportCommentService.CreateCommentAsync(
                commentViewModel,
                currentUser,
                User.Identity?.Name,
                canUseInternalComments);

            return RedirectToAction(nameof(Details), new { id = bugReport.Id });
        }

        // GET: BugReports/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateBugReportViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var bugReport = new BugReport
            {
                Title = viewModel.Title,
                Description = viewModel.Description,
                Priority = viewModel.Priority,
                Status = BugStatus.Open,
                CreatedByUserId = _userManager.GetUserId(User),
                CreateAt = DateTime.UtcNow
            };

            _context.Add(bugReport);
            await _context.SaveChangesAsync();
            await _bugReportAttachmentService.SaveAttachmentsAsync(bugReport.Id, viewModel.Attachments, bugReport.CreatedByUserId);

            return RedirectToAction(nameof(Details), new { id = bugReport.Id});
        }

        // GET: BugReports/Edit/5
        [Authorize(Roles = ApplicationRoles.AdminDeveloper)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bugReport = await _context.BugReports.FindAsync(id);
            if (bugReport == null)
            {
                return NotFound();
            }
            return View(bugReport);
        }

        // POST: BugReports/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ApplicationRoles.AdminDeveloper)]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Status,Priority,AssignedTo")] BugReport bugReport)
        {
            if (id != bugReport.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(bugReport);
            }

            var oldBugReport = await _context.BugReports
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == id);

            if (oldBugReport == null)
            {
                return NotFound();
            }

            var changes = _bugReportChangeService.GetChanges(oldBugReport, bugReport);
            foreach (var change in changes)
            {
                AddChangeLog(bugReport.Id, change, bugReport.AssignedTo);
            }

            bugReport.CreateAt = oldBugReport.CreateAt;
            bugReport.UpdatedAt = changes.Any() ? DateTime.UtcNow: oldBugReport.UpdatedAt;

            try
            {
                _context.Update(bugReport);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BugReportExists(bugReport.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction(nameof(Details), new { id = bugReport.Id });

        }


        // GET: BugReports/Delete/5
        [Authorize(Roles = ApplicationRoles.Admin)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bugReport = await _context.BugReports
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bugReport == null)
            {
                return NotFound();
            }

            return View(bugReport);

        }

        // POST: BugReports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ApplicationRoles.Admin)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bugReport = await _context.BugReports.FindAsync(id);
            if (bugReport != null)
            {
                _context.BugReports.Remove(bugReport);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> Search(BugStatus? status, BugPriority? priority, string? title, int page = 1)
        {
            var viewModel = await BuildIndexViewModel(status, priority, title, page);

            return PartialView("_BugReportTable", viewModel);
        }

        private bool BugReportExists(int id)
        {
            return _context.BugReports.Any(e => e.Id == id);
        }

        /// <summary>
        /// Create a Log entry for a single bug report change and adds it to the database context.
        /// </summary>
        /// <param name="bugReportId">The id for the bug report that was changed.</param>
        /// <param name="change">The detected field change that should be logged.</param>
        /// <param name="assignedTo">The user or developer currently assigned to the bug report.</param>
        private void AddChangeLog(int bugReportId, BugReportChange change, string? assignedTo)
        {
            BugReportLog log = new()
            {
                BugReportId = bugReportId,
                Message = $"{change.FieldName} changed from {change.OldValue} to {change.NewValue}",
                AssignedTo = assignedTo,
                CreatedAt = DateTime.UtcNow,
            };
            _context.BugReportLogs.Add(log);
        }

        /// <summary>
        /// Builds the view model for bug report index page.
        /// Applies search, filters, sorting and pagination.
        /// </summary>
        /// <param name="status">Optional status filter.</param>
        /// <param name="priority">Optional priority filter.</param>
        /// <param name="title">Optional Title search.</param>
        /// <param name="page">Current page number.</param>
        /// <returns>A prepared view model for the Index view.</returns>
        private async Task<BugReportIndexViewModel> BuildIndexViewModel(BugStatus? status, BugPriority? priority, string? title, int page)
        {
            const int pageSize = 10;
            page = Math.Max(page, 1);

            IQueryable<BugReport> bugReports = _context.BugReports;

            var canViewAllReports = User.IsInRole(ApplicationRoles.Admin) || User.IsInRole(ApplicationRoles.Developer);
            if (!canViewAllReports)
            {
                var currentUserId = _userManager.GetUserId(User);
                bugReports = bugReports.Where(b => b.CreatedByUserId == currentUserId);
            }

            if (!string.IsNullOrWhiteSpace(title))
            {
                bugReports = bugReports.Where(b => b.Title.Contains(title));
            }
            if (status.HasValue)
            {
                bugReports = bugReports.Where(b => b.Status == status.Value);
            }
            if (priority.HasValue)
            {
                bugReports = bugReports.Where(b => b.Priority == priority.Value);
            }


            var totalItems = await bugReports.CountAsync();
            var reports = await bugReports
                .OrderByDescending(b => b.UpdatedAt)
                .ThenByDescending(b => b.CreateAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new BugReportIndexViewModel
            {
                Reports = reports,
                SelectedStatus = status,
                SelectedPriority = priority,
                Search = title,
                Pagination = new PaginationViewModel
                {
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalItems = totalItems
                }
            };
        }
        private bool CanViewBugReport(BugReport bugReport)
        {
            var canViewAllReports = IsStaffUser();

            var currentUserId = _userManager.GetUserId(User);
            var isOwner = currentUserId == bugReport.CreatedByUserId;
            
            return canViewAllReports || isOwner;                

        }
        private bool IsStaffUser()
        {
            return User.IsInRole(ApplicationRoles.Admin) || User.IsInRole(ApplicationRoles.Developer);
        }
    }
}
