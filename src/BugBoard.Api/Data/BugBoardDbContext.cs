using BugBoard.Api.Models.Account;
using BugBoard.Api.Models.BugReports;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BugBoard.Api.Data;

public class BugBoardDbContext : IdentityDbContext<ApplicationUser>
{
    public BugBoardDbContext(DbContextOptions<BugBoardDbContext> opts) : base(opts) { 
    
    }

    public DbSet<BugReport> BugReports => Set<BugReport>();
    public DbSet<BugReportLog> BugReportLogs => Set<BugReportLog>();
    public DbSet<BugReportComment> BugReportComments => Set<BugReportComment>();
    public DbSet<BugReportAttachment> BugReportAttachments => Set<BugReportAttachment>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<BugReport>()
            .HasOne(b => b.AssignedToUser)
            .WithMany()
            .HasForeignKey(b => b.AssignedToId);

        builder.Entity<BugReportLog>()
            .HasOne(b => b.AssignedToUser)
            .WithMany()
            .HasForeignKey(b => b.AssignedToId);

        builder.Entity<BugReport>()
            .HasOne(s => s.Supervisor)
            .WithMany()
            .HasForeignKey(s =>  s.SupervisorId);
    }
}
