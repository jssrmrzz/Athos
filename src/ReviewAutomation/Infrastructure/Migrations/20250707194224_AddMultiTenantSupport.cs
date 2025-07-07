using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Athos.ReviewAutomation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMultiTenantSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BusinessId",
                table: "Reviews",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Businesses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    GoogleBusinessProfileId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    SubscriptionTier = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Businesses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GoogleId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ProfilePictureUrl = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BusinessOAuthTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BusinessId = table.Column<int>(type: "INTEGER", nullable: false),
                    Provider = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    AccessToken = table.Column<string>(type: "TEXT", nullable: false),
                    RefreshToken = table.Column<string>(type: "TEXT", nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsRevoked = table.Column<bool>(type: "INTEGER", nullable: false),
                    Scope = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessOAuthTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BusinessOAuthTokens_Businesses_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BusinessSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BusinessId = table.Column<int>(type: "INTEGER", nullable: false),
                    PreferredLlmProvider = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    DefaultResponseTemplate = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    AutoRespondToPositiveReviews = table.Column<bool>(type: "INTEGER", nullable: false),
                    EnableEmailNotifications = table.Column<bool>(type: "INTEGER", nullable: false),
                    EnableSmsNotifications = table.Column<bool>(type: "INTEGER", nullable: false),
                    SmsPhoneNumber = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    ReviewSyncIntervalMinutes = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BusinessSettings_Businesses_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BusinessUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BusinessId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    Role = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastAccessAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    InvitedByUserId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BusinessUsers_Businesses_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BusinessUsers_Users_InvitedByUserId",
                        column: x => x.InvitedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_BusinessUsers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_BusinessId_IsApproved",
                table: "Reviews",
                columns: new[] { "BusinessId", "IsApproved" });

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_BusinessId_Sentiment",
                table: "Reviews",
                columns: new[] { "BusinessId", "Sentiment" });

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_BusinessId_Timestamp",
                table: "Reviews",
                columns: new[] { "BusinessId", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_Businesses_GoogleBusinessProfileId",
                table: "Businesses",
                column: "GoogleBusinessProfileId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BusinessOAuthTokens_BusinessId_Provider",
                table: "BusinessOAuthTokens",
                columns: new[] { "BusinessId", "Provider" });

            migrationBuilder.CreateIndex(
                name: "IX_BusinessSettings_BusinessId",
                table: "BusinessSettings",
                column: "BusinessId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BusinessUsers_BusinessId_UserId",
                table: "BusinessUsers",
                columns: new[] { "BusinessId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BusinessUsers_InvitedByUserId",
                table: "BusinessUsers",
                column: "InvitedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessUsers_UserId",
                table: "BusinessUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_GoogleId",
                table: "Users",
                column: "GoogleId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Businesses_BusinessId",
                table: "Reviews",
                column: "BusinessId",
                principalTable: "Businesses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Businesses_BusinessId",
                table: "Reviews");

            migrationBuilder.DropTable(
                name: "BusinessOAuthTokens");

            migrationBuilder.DropTable(
                name: "BusinessSettings");

            migrationBuilder.DropTable(
                name: "BusinessUsers");

            migrationBuilder.DropTable(
                name: "Businesses");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_BusinessId_IsApproved",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_BusinessId_Sentiment",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_BusinessId_Timestamp",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "BusinessId",
                table: "Reviews");
        }
    }
}
