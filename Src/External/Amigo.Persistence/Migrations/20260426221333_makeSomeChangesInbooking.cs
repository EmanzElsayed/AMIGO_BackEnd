using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Amigo.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class makeSomeChangesInbooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SlotReservation_AvailableSlots_SlotId",
                schema: "tour",
                table: "SlotReservation");

            migrationBuilder.DropColumn(
                name: "RequiredTravelersCount",
                schema: "booking",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "TravelersCompleted",
                schema: "booking",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "ReservedCount",
                schema: "tour",
                table: "AvailableSlots");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                schema: "tour",
                table: "SlotReservation",
                type: "timestamp with time zone",
                nullable: true,
                defaultValueSql: "TIMEZONE('UTC', NOW())",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "tour",
                table: "SlotReservation",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                schema: "tour",
                table: "SlotReservation",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "TIMEZONE('UTC', NOW())",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "tour",
                table: "SlotReservation",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_SlotReservation_AvailableSlots_SlotId",
                schema: "tour",
                table: "SlotReservation",
                column: "SlotId",
                principalSchema: "tour",
                principalTable: "AvailableSlots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SlotReservation_AvailableSlots_SlotId",
                schema: "tour",
                table: "SlotReservation");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                schema: "tour",
                table: "SlotReservation",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true,
                oldDefaultValueSql: "TIMEZONE('UTC', NOW())");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "tour",
                table: "SlotReservation",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                schema: "tour",
                table: "SlotReservation",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "TIMEZONE('UTC', NOW())");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "tour",
                table: "SlotReservation",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

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
                name: "ReservedCount",
                schema: "tour",
                table: "AvailableSlots",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_SlotReservation_AvailableSlots_SlotId",
                schema: "tour",
                table: "SlotReservation",
                column: "SlotId",
                principalSchema: "tour",
                principalTable: "AvailableSlots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
