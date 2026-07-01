using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BugBoard.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddSupervisorIdToBugReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AssignedTo",
                table: "BugReports",
                newName: "SupervisorId");

            migrationBuilder.RenameColumn(
                name: "AssignedTo",
                table: "BugReportLogs",
                newName: "AssignedToId");

            migrationBuilder.AddColumn<string>(
                name: "AssignedToId",
                table: "BugReports",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BugReports_AssignedToId",
                table: "BugReports",
                column: "AssignedToId");

            migrationBuilder.CreateIndex(
                name: "IX_BugReports_SupervisorId",
                table: "BugReports",
                column: "SupervisorId");

            migrationBuilder.CreateIndex(
                name: "IX_BugReportLogs_AssignedToId",
                table: "BugReportLogs",
                column: "AssignedToId");

            migrationBuilder.AddForeignKey(
                name: "FK_BugReportLogs_AspNetUsers_AssignedToId",
                table: "BugReportLogs",
                column: "AssignedToId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BugReports_AspNetUsers_AssignedToId",
                table: "BugReports",
                column: "AssignedToId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BugReports_AspNetUsers_SupervisorId",
                table: "BugReports",
                column: "SupervisorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BugReportLogs_AspNetUsers_AssignedToId",
                table: "BugReportLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_BugReports_AspNetUsers_AssignedToId",
                table: "BugReports");

            migrationBuilder.DropForeignKey(
                name: "FK_BugReports_AspNetUsers_SupervisorId",
                table: "BugReports");

            migrationBuilder.DropIndex(
                name: "IX_BugReports_AssignedToId",
                table: "BugReports");

            migrationBuilder.DropIndex(
                name: "IX_BugReports_SupervisorId",
                table: "BugReports");

            migrationBuilder.DropIndex(
                name: "IX_BugReportLogs_AssignedToId",
                table: "BugReportLogs");

            migrationBuilder.DropColumn(
                name: "AssignedToId",
                table: "BugReports");

            migrationBuilder.RenameColumn(
                name: "SupervisorId",
                table: "BugReports",
                newName: "AssignedTo");

            migrationBuilder.RenameColumn(
                name: "AssignedToId",
                table: "BugReportLogs",
                newName: "AssignedTo");
        }
    }
}
