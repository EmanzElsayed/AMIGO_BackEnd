using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Amigo.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class removeVoucherEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Voucher",
                schema: "booking");

            migrationBuilder.AddColumn<string>(
                name: "VoucherToken",
                schema: "booking",
                table: "Booking",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VoucherToken",
                schema: "booking",
                table: "Booking");

            migrationBuilder.CreateTable(
                name: "Voucher",
                schema: "booking",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    ModifiedBy = table.Column<int>(type: "integer", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    BookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsSentByEmail = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IssuedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    QRCodeBase64 = table.Column<string>(type: "text", nullable: false),
                    SentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Token = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    VoucherNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Voucher", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Voucher_Booking_BookingId",
                        column: x => x.BookingId,
                        principalSchema: "booking",
                        principalTable: "Booking",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Voucher_BookingId",
                schema: "booking",
                table: "Voucher",
                column: "BookingId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Voucher_Token",
                schema: "booking",
                table: "Voucher",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Voucher_VoucherNumber",
                schema: "booking",
                table: "Voucher",
                column: "VoucherNumber");
        }
    }
}
