using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Athos.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddApprovedAtToDbReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                table: "Reviews",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                table: "Reviews");
        }
    }
}
