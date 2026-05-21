using BugBoard.Api.Data;
using BugBoard.Api.Models.BugReports;
using BugBoard.Api.Services.BugReports;
using BugBoard.Api.ViewModels.BugReports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BugBoard.Api.Controllers
{
    [Authorize]
    public class BugReportsController : Controller
    {
        private readonly BugBoardDbContext _context;
        private readonly BugReportChangeService _bugReportChangeService;
        public BugReportsController(BugBoardDbContext context, BugReportChangeService bugReportChangeService)
        {
            _context = context;
            _bugReportChangeService = bugReportChangeService;
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
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bugReport == null)
            {
                return NotFound();
            }

            return View(bugReport);
        }

        // GET: BugReports/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BugReports/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Status,Priority,AssignedTo,CreateAt")] BugReport bugReport)
        {
            if (ModelState.IsValid)
            {
                bugReport.CreateAt = DateTime.Now;

                _context.Add(bugReport);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(bugReport);
        }

        // GET: BugReports/Edit/5
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
            bugReport.UpdatedAt = DateTime.Now;
            return View(bugReport);
        }

        // POST: BugReports/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
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
            bugReport.UpdatedAt = changes.Any() ? DateTime.Now : oldBugReport.UpdatedAt;

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


        private void AddChangeLog(int bugReportId, BugReportChange change, string? assignedTo)
        {
            BugReportLog log = new()
            {
                BugReportId = bugReportId,
                Message = $"{change.FieldName} changed from {change.OldValue} to {change.NewValue}",
                AssignedTo = assignedTo,
                CreatedAt = DateTime.Now,
            };
            _context.BugReportLogs.Add(log);
        }


        // Builds the view model for bug report index page
        //param status   = optional stauts filter
        //param priority = optional priority filter
        //param title    = optional title search term
        //param page     = current page number
        //return         = prepared view model for index view
        private async Task<BugReportIndexViewModel> BuildIndexViewModel(BugStatus? status, BugPriority? priority, string? title, int page)
        {
            const int pageSize = 10;
            page = Math.Max(page, 1);

            IQueryable<BugReport> bugReports = _context.BugReports;

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
    }
}
