using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Amigo.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ApplyNewChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancelationPolicyType",
                schema: "booking",
                table: "OrderItem");

            migrationBuilder.DropColumn(
                name: "CancellationBefore",
                schema: "booking",
                table: "OrderItem");

            migrationBuilder.DropColumn(
                name: "RefundPercentage",
                schema: "booking",
                table: "OrderItem");

            migrationBuilder.AlterColumn<Guid>(
                name: "SlotId",
                schema: "tour",
                table: "SlotReservation",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<DateTime>(
                name: "TourDateTime",
                schema: "tour",
                table: "SlotReservation",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsSpecialDate",
                schema: "booking",
                table: "OrderItem",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<Guid>(
                name: "SlotId",
                schema: "booking",
                table: "CartItem",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<bool>(
                name: "IsSpecialDate",
                schema: "booking",
                table: "CartItem",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "OrderItemCancellationPolicy",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<int>(type: "integer", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    OrderItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    CancelationPolicyType = table.Column<int>(type: "integer", nullable: false),
                    CancellationBefore = table.Column<TimeSpan>(type: "interval", nullable: false),
                    RefundPercentage = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItemCancellationPolicy", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItemCancellationPolicy_OrderItem_OrderItemId",
                        column: x => x.OrderItemId,
                        principalSchema: "booking",
                        principalTable: "OrderItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemCancellationPolicy_OrderItemId",
                table: "OrderItemCancellationPolicy",
                column: "OrderItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderItemCancellationPolicy");

            migrationBuilder.DropColumn(
                name: "TourDateTime",
                schema: "tour",
                table: "SlotReservation");

            migrationBuilder.DropColumn(
                name: "IsSpecialDate",
                schema: "booking",
                table: "OrderItem");

            migrationBuilder.DropColumn(
                name: "IsSpecialDate",
                schema: "booking",
                table: "CartItem");

            migrationBuilder.AlterColumn<Guid>(
                name: "SlotId",
                schema: "tour",
                table: "SlotReservation",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CancelationPolicyType",
                schema: "booking",
                table: "OrderItem",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "CancellationBefore",
                schema: "booking",
                table: "OrderItem",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<decimal>(
                name: "RefundPercentage",
                schema: "booking",
                table: "OrderItem",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<Guid>(
                name: "SlotId",
                schema: "booking",
                table: "CartItem",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);
        }
    }
}
