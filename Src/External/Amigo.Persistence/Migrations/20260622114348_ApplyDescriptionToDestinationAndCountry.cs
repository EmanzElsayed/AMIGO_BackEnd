using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Amigo.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ApplyDescriptionToDestinationAndCountry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "translation",
                table: "DestinationTranslation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "translation",
                table: "CountryInfoTranslation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                schema: "public",
                table: "CountryInfo",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                schema: "public",
                table: "CountryInfo",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                schema: "translation",
                table: "DestinationTranslation");

            migrationBuilder.DropColumn(
                name: "Description",
                schema: "translation",
                table: "CountryInfoTranslation");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                schema: "public",
                table: "CountryInfo");

            migrationBuilder.DropColumn(
                name: "PublicId",
                schema: "public",
                table: "CountryInfo");
        }
    }
}
