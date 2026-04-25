using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Amigo.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChangeProviderPaymentIntentIdToPaymentProfiderReferenceId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProviderPaymentIntentId",
                schema: "payment",
                table: "Payment",
                newName: "PaymentProviderReferenceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PaymentProviderReferenceId",
                schema: "payment",
                table: "Payment",
                newName: "ProviderPaymentIntentId");
        }
    }
}
