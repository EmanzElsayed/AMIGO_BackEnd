using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Amigo.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MakeSomeChangesInbookingAndPayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FailureReason",
                schema: "payment",
                table: "Payment",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdempotencyKey",
                schema: "payment",
                table: "Payment",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentProviderReferenceId",
                schema: "payment",
                table: "Payment",
                type: "text",
                nullable: true);
            migrationBuilder.RenameColumn(
                name: "TourName",
                schema: "booking",
                table: "OrderItem",
                newName: "TourTitle");
            migrationBuilder.AlterColumn<string>(
                name: "TourTitle",
                schema: "booking",
                table: "OrderItem",
                type: "character varying(400)",
                maxLength: 400,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(250)",
                oldMaxLength: 250);

            migrationBuilder.AlterColumn<string>(
                name: "DestinationName",
                schema: "booking",
                table: "OrderItem",
                type: "character varying(300)",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(250)",
                oldMaxLength: 250);
            migrationBuilder.RenameColumn(
              name: "TourName",
              schema: "booking",
              table: "CartItem",
              newName: "TourTitle");

            migrationBuilder.AlterColumn<string>(
                name: "TourTitle",
                schema: "booking",
                table: "CartItem",
                type: "character varying(400)",
                maxLength: 400,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(250)",
                oldMaxLength: 250);

            migrationBuilder.AlterColumn<string>(
                name: "DestinationName",
                schema: "booking",
                table: "CartItem",
                type: "character varying(300)",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(250)",
                oldMaxLength: 250);

            migrationBuilder.AddColumn<Guid>(
                name: "OrderId",
                schema: "booking",
                table: "Booking",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "PaymentId",
                schema: "booking",
                table: "Booking",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "RequiredTravelersCount",
                schema: "booking",
                table: "Booking",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "TravelersCompleted",
                schema: "booking",
                table: "Booking",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "BookedCount",
                schema: "tour",
                table: "AvailableSlots",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReservedCount",
                schema: "tour",
                table: "AvailableSlots",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FailureReason",
                schema: "payment",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "IdempotencyKey",
                schema: "payment",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "PaymentProviderReferenceId",
                schema: "payment",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "OrderId",
                schema: "booking",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "PaymentId",
                schema: "booking",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "RequiredTravelersCount",
                schema: "booking",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "TravelersCompleted",
                schema: "booking",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "BookedCount",
                schema: "tour",
                table: "AvailableSlots");

            migrationBuilder.DropColumn(
                name: "ReservedCount",
                schema: "tour",
                table: "AvailableSlots");

            migrationBuilder.AlterColumn<string>(
                name: "TourTitle",
                schema: "booking",
                table: "OrderItem",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(400)",
                oldMaxLength: 400);

            migrationBuilder.AlterColumn<string>(
                name: "DestinationName",
                schema: "booking",
                table: "OrderItem",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(300)",
                oldMaxLength: 300);

            migrationBuilder.AlterColumn<string>(
                name: "TourTitle",
                schema: "booking",
                table: "CartItem",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(400)",
                oldMaxLength: 400);

            migrationBuilder.AlterColumn<string>(
                name: "DestinationName",
                schema: "booking",
                table: "CartItem",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(300)",
                oldMaxLength: 300);
        }
    }
}
