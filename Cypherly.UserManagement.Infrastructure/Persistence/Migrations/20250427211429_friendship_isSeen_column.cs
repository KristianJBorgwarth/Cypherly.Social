using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cypherly.UserManagement.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class friendship_isSeen_column : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSeen",
                table: "Friendship",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_BlockedUser_BlockedUserProfileId",
                table: "BlockedUser",
                column: "BlockedUserProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_BlockedUser_UserProfile_BlockedUserProfileId",
                table: "BlockedUser",
                column: "BlockedUserProfileId",
                principalTable: "UserProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlockedUser_UserProfile_BlockedUserProfileId",
                table: "BlockedUser");

            migrationBuilder.DropIndex(
                name: "IX_BlockedUser_BlockedUserProfileId",
                table: "BlockedUser");

            migrationBuilder.DropColumn(
                name: "IsSeen",
                table: "Friendship");
        }
    }
}
