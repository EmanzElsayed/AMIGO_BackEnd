using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Amigo.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateVoucher : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PdfUrl",
                schema: "booking",
                table: "Voucher");

            migrationBuilder.DropColumn(
                name: "QRCode",
                schema: "booking",
                table: "Voucher");

            migrationBuilder.AlterColumn<bool>(
                name: "IsSentByEmail",
                schema: "booking",
                table: "Voucher",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AddColumn<string>(
                name: "QRCodeBase64",
                schema: "booking",
                table: "Voucher",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                schema: "booking",
                table: "Voucher",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Token",
                schema: "booking",
                table: "Voucher",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsVoucherCreated",
                schema: "booking",
                table: "Booking",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "VoucherId",
                schema: "booking",
                table: "Booking",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Voucher_Token",
                schema: "booking",
                table: "Voucher",
                column: "Token",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Voucher_Token",
                schema: "booking",
                table: "Voucher");

            migrationBuilder.DropColumn(
                name: "QRCodeBase64",
                schema: "booking",
                table: "Voucher");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "booking",
                table: "Voucher");

            migrationBuilder.DropColumn(
                name: "Token",
                schema: "booking",
                table: "Voucher");

            migrationBuilder.DropColumn(
                name: "IsVoucherCreated",
                schema: "booking",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "VoucherId",
                schema: "booking",
                table: "Booking");

            migrationBuilder.AlterColumn<bool>(
                name: "IsSentByEmail",
                schema: "booking",
                table: "Voucher",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PdfUrl",
                schema: "booking",
                table: "Voucher",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QRCode",
                schema: "booking",
                table: "Voucher",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);
        }
    }
}
