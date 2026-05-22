using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BugBoard.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedByUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "BugReports",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BugReports_CreatedByUserId",
                table: "BugReports",
                column: "CreatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BugReports_AspNetUsers_CreatedByUserId",
                table: "BugReports",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BugReports_AspNetUsers_CreatedByUserId",
                table: "BugReports");

            migrationBuilder.DropIndex(
                name: "IX_BugReports_CreatedByUserId",
                table: "BugReports");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "BugReports");
        }
    }
}
