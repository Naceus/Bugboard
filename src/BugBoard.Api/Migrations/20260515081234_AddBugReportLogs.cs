using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BugBoard.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddBugReportLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BugReportLog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Message = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AssignedTo = table.Column<string>(type: "TEXT", nullable: true),
                    BugReportId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BugReportLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BugReportLog_BugReports_BugReportId",
                        column: x => x.BugReportId,
                        principalTable: "BugReports",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BugReportLog_BugReportId",
                table: "BugReportLog",
                column: "BugReportId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BugReportLog");
        }
    }
}
