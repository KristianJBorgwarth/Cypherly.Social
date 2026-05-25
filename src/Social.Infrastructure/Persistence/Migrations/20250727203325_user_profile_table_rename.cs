using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Social.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class user_profile_table_rename : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_blocked_user_UserProfile_BlockedUserProfileId",
                table: "blocked_user");

            migrationBuilder.DropForeignKey(
                name: "FK_blocked_user_UserProfile_blocking_user_profile_id",
                table: "blocked_user");

            migrationBuilder.DropForeignKey(
                name: "FK_friendship_UserProfile_friend_profile_id",
                table: "friendship");

            migrationBuilder.DropForeignKey(
                name: "FK_friendship_UserProfile_user_profile_id",
                table: "friendship");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserProfile",
                table: "UserProfile");

            migrationBuilder.RenameTable(
                name: "UserProfile",
                newName: "user_profile");

            migrationBuilder.AddPrimaryKey(
                name: "PK_user_profile",
                table: "user_profile",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_blocked_user_user_profile_BlockedUserProfileId",
                table: "blocked_user",
                column: "BlockedUserProfileId",
                principalTable: "user_profile",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_blocked_user_user_profile_blocking_user_profile_id",
                table: "blocked_user",
                column: "blocking_user_profile_id",
                principalTable: "user_profile",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_friendship_user_profile_friend_profile_id",
                table: "friendship",
                column: "friend_profile_id",
                principalTable: "user_profile",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_friendship_user_profile_user_profile_id",
                table: "friendship",
                column: "user_profile_id",
                principalTable: "user_profile",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_blocked_user_user_profile_BlockedUserProfileId",
                table: "blocked_user");

            migrationBuilder.DropForeignKey(
                name: "FK_blocked_user_user_profile_blocking_user_profile_id",
                table: "blocked_user");

            migrationBuilder.DropForeignKey(
                name: "FK_friendship_user_profile_friend_profile_id",
                table: "friendship");

            migrationBuilder.DropForeignKey(
                name: "FK_friendship_user_profile_user_profile_id",
                table: "friendship");

            migrationBuilder.DropPrimaryKey(
                name: "PK_user_profile",
                table: "user_profile");

            migrationBuilder.RenameTable(
                name: "user_profile",
                newName: "UserProfile");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserProfile",
                table: "UserProfile",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_blocked_user_UserProfile_BlockedUserProfileId",
                table: "blocked_user",
                column: "BlockedUserProfileId",
                principalTable: "UserProfile",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_blocked_user_UserProfile_blocking_user_profile_id",
                table: "blocked_user",
                column: "blocking_user_profile_id",
                principalTable: "UserProfile",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_friendship_UserProfile_friend_profile_id",
                table: "friendship",
                column: "friend_profile_id",
                principalTable: "UserProfile",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_friendship_UserProfile_user_profile_id",
                table: "friendship",
                column: "user_profile_id",
                principalTable: "UserProfile",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
