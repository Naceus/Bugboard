using BugBoard.Api.Data;
using BugBoard.Api.Data.Seed;
using BugBoard.Api.Models.Account;
using BugBoard.Api.Services.BugReports;
using BugBoard.Api.Services.Dashboard;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services
            .AddControllersWithViews();
        builder.Services
            .AddDbContext<BugBoardDbContext>(options => options
            .UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
        builder.Services
            .AddScoped<BugReportChangeService>();
        builder.Services
            .AddScoped<IBugReportCommentService, BugReportCommentService>();
        builder.Services
            .AddScoped<IDashboardService, DashboardService>();
        builder.Services
            .AddScoped<DatabaseSeeder>();
        //Add Identity for login
        builder.Services
            .AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<BugBoardDbContext>()
            .AddDefaultTokenProviders();

        builder.Services
            .Configure<DataProtectionTokenProviderOptions>
            (options => options.TokenLifespan = TimeSpan.FromMinutes(15));
        var app = builder.Build();

        using(var scope = app.Services.CreateScope())
        {
            var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
            seeder.SeedAsync().GetAwaiter().GetResult();
        }
        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();
        
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}
