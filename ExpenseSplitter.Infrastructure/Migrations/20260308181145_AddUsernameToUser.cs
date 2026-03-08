using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseSplitter.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUsernameToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExpenseParticipants_Users_UserId",
                table: "ExpenseParticipants");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupMembers_Users_UserId",
                table: "GroupMembers");

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_ExpenseParticipants_Users_UserId",
                table: "ExpenseParticipants",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupMembers_Users_UserId",
                table: "GroupMembers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExpenseParticipants_Users_UserId",
                table: "ExpenseParticipants");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupMembers_Users_UserId",
                table: "GroupMembers");

            migrationBuilder.DropColumn(
                name: "Username",
                table: "Users");

            migrationBuilder.AddForeignKey(
                name: "FK_ExpenseParticipants_Users_UserId",
                table: "ExpenseParticipants",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupMembers_Users_UserId",
                table: "GroupMembers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
