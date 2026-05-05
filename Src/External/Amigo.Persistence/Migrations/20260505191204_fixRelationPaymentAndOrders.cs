using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Amigo.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class fixRelationPaymentAndOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payment_Order_OrderId1",
                schema: "payment",
                table: "Payment");

            migrationBuilder.DropIndex(
                name: "IX_Payment_OrderId1",
                schema: "payment",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "OrderId1",
                schema: "payment",
                table: "Payment");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OrderId1",
                schema: "payment",
                table: "Payment",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payment_OrderId1",
                schema: "payment",
                table: "Payment",
                column: "OrderId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_Order_OrderId1",
                schema: "payment",
                table: "Payment",
                column: "OrderId1",
                principalSchema: "booking",
                principalTable: "Order",
                principalColumn: "Id");
        }
    }
}
