using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Amigo.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ApplyCancellationPolicyAndRefundPercentage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "RefundPercentage",
                schema: "booking",
                table: "CancellationRequest",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "cancelationPolicyType",
                schema: "booking",
                table: "CancellationRequest",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefundPercentage",
                schema: "booking",
                table: "CancellationRequest");

            migrationBuilder.DropColumn(
                name: "cancelationPolicyType",
                schema: "booking",
                table: "CancellationRequest");
        }
    }
}
