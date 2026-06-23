using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Amigo.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class applyCapitalAndLanguageToCountryInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Capital",
                schema: "translation",
                table: "CountryInfoTranslation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OfficialLanguage",
                schema: "translation",
                table: "CountryInfoTranslation",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Capital",
                schema: "translation",
                table: "CountryInfoTranslation");

            migrationBuilder.DropColumn(
                name: "OfficialLanguage",
                schema: "translation",
                table: "CountryInfoTranslation");
        }
    }
}
