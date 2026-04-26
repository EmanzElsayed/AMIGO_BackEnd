using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Amigo.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTravelersDraft : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Type",
                schema: "booking",
                table: "Traveler",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CommentForProvider",
                schema: "booking",
                table: "OrderItem",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameAndAddressOfAccomodation",
                schema: "booking",
                table: "OrderItem",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CommentForProvider",
                schema: "booking",
                table: "Booking",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameAndAddressOfAccomodation",
                schema: "booking",
                table: "Booking",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TravelerDraft",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    ModifiedBy = table.Column<int>(type: "integer", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    OrderItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    FullName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Nationality = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    BirthDate = table.Column<DateOnly>(type: "date", nullable: true),
                    PassportNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PhoneNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TravelerDraft", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TravelerDraft_OrderItem_OrderItemId",
                        column: x => x.OrderItemId,
                        principalSchema: "booking",
                        principalTable: "OrderItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TravelerDraft_OrderItemId",
                table: "TravelerDraft",
                column: "OrderItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TravelerDraft");

            migrationBuilder.DropColumn(
                name: "Type",
                schema: "booking",
                table: "Traveler");

            migrationBuilder.DropColumn(
                name: "CommentForProvider",
                schema: "booking",
                table: "OrderItem");

            migrationBuilder.DropColumn(
                name: "NameAndAddressOfAccomodation",
                schema: "booking",
                table: "OrderItem");

            migrationBuilder.DropColumn(
                name: "CommentForProvider",
                schema: "booking",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "NameAndAddressOfAccomodation",
                schema: "booking",
                table: "Booking");
        }
    }
}
