using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Amigo.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenameCreatevoucherToSendEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsVoucherCreated",
                schema: "booking",
                table: "Booking",
                newName: "IsVoucherSentByEmail");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsVoucherSentByEmail",
                schema: "booking",
                table: "Booking",
                newName: "IsVoucherCreated");
        }
    }
}
