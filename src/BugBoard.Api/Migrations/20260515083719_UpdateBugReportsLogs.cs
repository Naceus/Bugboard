using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BugBoard.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBugReportsLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BugReportLog_BugReports_BugReportId",
                table: "BugReportLog");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BugReportLog",
                table: "BugReportLog");

            migrationBuilder.RenameTable(
                name: "BugReportLog",
                newName: "BugReportLogs");

            migrationBuilder.RenameIndex(
                name: "IX_BugReportLog_BugReportId",
                table: "BugReportLogs",
                newName: "IX_BugReportLogs_BugReportId");

            migrationBuilder.AlterColumn<int>(
                name: "BugReportId",
                table: "BugReportLogs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_BugReportLogs",
                table: "BugReportLogs",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BugReportLogs_BugReports_BugReportId",
                table: "BugReportLogs",
                column: "BugReportId",
                principalTable: "BugReports",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BugReportLogs_BugReports_BugReportId",
                table: "BugReportLogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BugReportLogs",
                table: "BugReportLogs");

            migrationBuilder.RenameTable(
                name: "BugReportLogs",
                newName: "BugReportLog");

            migrationBuilder.RenameIndex(
                name: "IX_BugReportLogs_BugReportId",
                table: "BugReportLog",
                newName: "IX_BugReportLog_BugReportId");

            migrationBuilder.AlterColumn<int>(
                name: "BugReportId",
                table: "BugReportLog",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BugReportLog",
                table: "BugReportLog",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BugReportLog_BugReports_BugReportId",
                table: "BugReportLog",
                column: "BugReportId",
                principalTable: "BugReports",
                principalColumn: "Id");
        }
    }
}
