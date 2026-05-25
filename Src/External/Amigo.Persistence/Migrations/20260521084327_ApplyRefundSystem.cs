using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Amigo.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ApplyRefundSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BookingId",
                schema: "booking",
                table: "Refund",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CancellationRequestId",
                schema: "booking",
                table: "Refund",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "FailureReason",
                schema: "booking",
                table: "Refund",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdempotencyKey",
                schema: "booking",
                table: "Refund",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProviderResponseJson",
                schema: "booking",
                table: "Refund",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProviderCaptureId",
                schema: "payment",
                table: "Payment",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdminNotes",
                schema: "booking",
                table: "CancellationRequest",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Refund_BookingId",
                schema: "booking",
                table: "Refund",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_Refund_CancellationRequestId",
                schema: "booking",
                table: "Refund",
                column: "CancellationRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_Refund_Booking_BookingId",
                schema: "booking",
                table: "Refund",
                column: "BookingId",
                principalSchema: "booking",
                principalTable: "Booking",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Refund_CancellationRequest_CancellationRequestId",
                schema: "booking",
                table: "Refund",
                column: "CancellationRequestId",
                principalSchema: "booking",
                principalTable: "CancellationRequest",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Refund_Booking_BookingId",
                schema: "booking",
                table: "Refund");

            migrationBuilder.DropForeignKey(
                name: "FK_Refund_CancellationRequest_CancellationRequestId",
                schema: "booking",
                table: "Refund");

            migrationBuilder.DropIndex(
                name: "IX_Refund_BookingId",
                schema: "booking",
                table: "Refund");

            migrationBuilder.DropIndex(
                name: "IX_Refund_CancellationRequestId",
                schema: "booking",
                table: "Refund");

            migrationBuilder.DropColumn(
                name: "BookingId",
                schema: "booking",
                table: "Refund");

            migrationBuilder.DropColumn(
                name: "CancellationRequestId",
                schema: "booking",
                table: "Refund");

            migrationBuilder.DropColumn(
                name: "FailureReason",
                schema: "booking",
                table: "Refund");

            migrationBuilder.DropColumn(
                name: "IdempotencyKey",
                schema: "booking",
                table: "Refund");

            migrationBuilder.DropColumn(
                name: "ProviderResponseJson",
                schema: "booking",
                table: "Refund");

            migrationBuilder.DropColumn(
                name: "ProviderCaptureId",
                schema: "payment",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "AdminNotes",
                schema: "booking",
                table: "CancellationRequest");
        }
    }
}
