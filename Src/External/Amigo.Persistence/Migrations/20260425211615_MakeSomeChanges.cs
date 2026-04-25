using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Amigo.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MakeSomeChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BookedCount",
                schema: "tour",
                table: "AvailableSlots");

          

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiresAt",
                schema: "booking",
                table: "Order",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpiresAt",
                schema: "booking",
                table: "Order");

          

            migrationBuilder.AddColumn<int>(
                name: "BookedCount",
                schema: "tour",
                table: "AvailableSlots",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
