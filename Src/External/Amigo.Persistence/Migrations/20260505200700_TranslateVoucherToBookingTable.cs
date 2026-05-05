using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Amigo.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class TranslateVoucherToBookingTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "QRCodeBase64",
                schema: "booking",
                table: "Booking",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "VoucherSentAt",
                schema: "booking",
                table: "Booking",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QRCodeBase64",
                schema: "booking",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "VoucherSentAt",
                schema: "booking",
                table: "Booking");
        }
    }
}
