using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Amigo.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ApPlyNewChangesInToursSection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Price_TourId_Type",
                schema: "booking",
                table: "Price");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                schema: "tour",
                table: "Tour");

            migrationBuilder.DropColumn(
                name: "IsVip",
                schema: "tour",
                table: "Tour");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                schema: "booking",
                table: "Price");

            migrationBuilder.DropColumn(
                name: "IsVip",
                schema: "booking",
                table: "Price");

            migrationBuilder.DropColumn(
                name: "Type",
                schema: "booking",
                table: "Price");

            migrationBuilder.DropColumn(
                name: "HoursBefore",
                schema: "tour",
                table: "Cancellation");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "EndDate",
                schema: "tour",
                table: "TourSchedule",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AddColumn<int>(
                name: "UserType",
                schema: "tour",
                table: "Tour",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserType",
                schema: "booking",
                table: "Price",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "CancellationBefore",
                schema: "tour",
                table: "Cancellation",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AlterColumn<TimeOnly>(
                name: "EndTime",
                schema: "tour",
                table: "AvailableSlots",
                type: "time",
                nullable: true,
                oldClrType: typeof(TimeOnly),
                oldType: "time");

            migrationBuilder.CreateTable(
                name: "PriceTranslation",
                schema: "tour",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    ModifiedBy = table.Column<int>(type: "integer", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    PriceId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                    Language = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceTranslation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PriceTranslation_Price_PriceId",
                        column: x => x.PriceId,
                        principalSchema: "booking",
                        principalTable: "Price",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Price_TourId_UserType",
                schema: "booking",
                table: "Price",
                columns: new[] { "TourId", "UserType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PriceTranslation_Language",
                schema: "tour",
                table: "PriceTranslation",
                column: "Language");

            migrationBuilder.CreateIndex(
                name: "IX_PriceTranslation_PriceId_Language",
                schema: "tour",
                table: "PriceTranslation",
                columns: new[] { "PriceId", "Language" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PriceTranslation",
                schema: "tour");

            migrationBuilder.DropIndex(
                name: "IX_Price_TourId_UserType",
                schema: "booking",
                table: "Price");

            migrationBuilder.DropColumn(
                name: "UserType",
                schema: "tour",
                table: "Tour");

            migrationBuilder.DropColumn(
                name: "UserType",
                schema: "booking",
                table: "Price");

            migrationBuilder.DropColumn(
                name: "CancellationBefore",
                schema: "tour",
                table: "Cancellation");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "EndDate",
                schema: "tour",
                table: "TourSchedule",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1),
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                schema: "tour",
                table: "Tour",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsVip",
                schema: "tour",
                table: "Tour",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                schema: "booking",
                table: "Price",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsVip",
                schema: "booking",
                table: "Price",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                schema: "booking",
                table: "Price",
                type: "character varying(400)",
                maxLength: 400,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<TimeOnly>(
                name: "HoursBefore",
                schema: "tour",
                table: "Cancellation",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AlterColumn<TimeOnly>(
                name: "EndTime",
                schema: "tour",
                table: "AvailableSlots",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0),
                oldClrType: typeof(TimeOnly),
                oldType: "time",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Price_TourId_Type",
                schema: "booking",
                table: "Price",
                columns: new[] { "TourId", "Type" },
                unique: true);
        }
    }
}
