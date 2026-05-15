using BugBoard.Api.Models.BugReports;
using Microsoft.EntityFrameworkCore;

namespace BugBoard.Api.Data;

public class BugBoardDbContext : DbContext
{
    public BugBoardDbContext(DbContextOptions<BugBoardDbContext> opts) : base(opts) { }

    public DbSet<BugReport> BugReports => Set<BugReport>();
    public DbSet<BugReportLog> BugReportLogs => Set<BugReportLog>();
}
