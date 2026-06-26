using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BugBoard.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddBugReportComment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BugReportComments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Comment = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedByName = table.Column<string>(type: "TEXT", nullable: false),
                    BugReportId = table.Column<int>(type: "INTEGER", nullable: false),
                    CommentVisibility = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BugReportComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BugReportComments_AspNetUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BugReportComments_BugReports_BugReportId",
                        column: x => x.BugReportId,
                        principalTable: "BugReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BugReportComments_BugReportId",
                table: "BugReportComments",
                column: "BugReportId");

            migrationBuilder.CreateIndex(
                name: "IX_BugReportComments_CreatedByUserId",
                table: "BugReportComments",
                column: "CreatedByUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BugReportComments");
        }
    }
}
