using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Athos.ReviewAutomation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSubmittedAtToDbReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "SubmittedAt",
                table: "Reviews",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubmittedAt",
                table: "Reviews");
        }
    }
}
