using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Amigo.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddIsPivAndIsPublicAndApplyCurrency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrencyCode",
                schema: "tour",
                table: "Tour",
                type: "integer",
                nullable: false,
                defaultValue: 0);

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrencyCode",
                schema: "tour",
                table: "Tour");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                schema: "tour",
                table: "Tour");

            migrationBuilder.DropColumn(
                name: "IsVip",
                schema: "tour",
                table: "Tour");
        }
    }
}
