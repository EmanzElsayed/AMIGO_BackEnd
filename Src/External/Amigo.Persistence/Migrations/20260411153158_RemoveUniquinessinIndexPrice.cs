using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Amigo.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUniquinessinIndexPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Price_TourId_UserType",
                schema: "booking",
                table: "Price");

            migrationBuilder.CreateIndex(
                name: "IX_Price_TourId_UserType",
                schema: "booking",
                table: "Price",
                columns: new[] { "TourId", "UserType" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Price_TourId_UserType",
                schema: "booking",
                table: "Price");

            migrationBuilder.CreateIndex(
                name: "IX_Price_TourId_UserType",
                schema: "booking",
                table: "Price",
                columns: new[] { "TourId", "UserType" },
                unique: true);
        }
    }
}
