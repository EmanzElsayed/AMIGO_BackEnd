using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Amigo.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class applyBaseAmountToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmountWithUsd",
                schema: "payment",
                table: "Payment",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmountWithUsd",
                schema: "booking",
                table: "Order",
                type: "numeric",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalAmountWithUsd",
                schema: "payment",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "TotalAmountWithUsd",
                schema: "booking",
                table: "Order");
        }
    }
}
