using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Social.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class up_avatar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_blocked_user_user_profile_BlockedUserProfileId",
                table: "blocked_user");

            migrationBuilder.RenameColumn(
                name: "BlockedUserProfileId",
                table: "blocked_user",
                newName: "blocked_user_profile_id");

            migrationBuilder.RenameIndex(
                name: "IX_blocked_user_BlockedUserProfileId",
                table: "blocked_user",
                newName: "IX_blocked_user_blocked_user_profile_id");

            migrationBuilder.CreateTable(
                name: "avatar",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_profile_id = table.Column<Guid>(type: "uuid", nullable: false),
                    e_tag = table.Column<string>(type: "text", nullable: false),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Deleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_avatar", x => x.id);
                    table.ForeignKey(
                        name: "FK_avatar_user_profile_user_profile_id",
                        column: x => x.user_profile_id,
                        principalTable: "user_profile",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_avatar_user_profile_id",
                table: "avatar",
                column: "user_profile_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_blocked_user_user_profile_blocked_user_profile_id",
                table: "blocked_user",
                column: "blocked_user_profile_id",
                principalTable: "user_profile",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_blocked_user_user_profile_blocked_user_profile_id",
                table: "blocked_user");

            migrationBuilder.DropTable(
                name: "avatar");

            migrationBuilder.RenameColumn(
                name: "blocked_user_profile_id",
                table: "blocked_user",
                newName: "BlockedUserProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_blocked_user_blocked_user_profile_id",
                table: "blocked_user",
                newName: "IX_blocked_user_BlockedUserProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_blocked_user_user_profile_BlockedUserProfileId",
                table: "blocked_user",
                column: "BlockedUserProfileId",
                principalTable: "user_profile",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
