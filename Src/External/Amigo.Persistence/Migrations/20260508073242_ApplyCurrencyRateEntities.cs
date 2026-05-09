using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Amigo.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ApplyCurrencyRateEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
               name: "RetailPrice",
               schema: "booking",
               table: "CartPrice",
               newName: "RetailPrice");

            migrationBuilder.RenameColumn(
              name: "RetailPrice",
              schema: "booking",
              table: "OrderedPrice",
              newName: "RetailPrice");

            migrationBuilder.DropColumn(
                name: "CurrencyCode",
                schema: "booking",
                table: "OrderItem");

            migrationBuilder.AlterColumn<decimal>(
                name: "RetailPrice",
                schema: "booking",
                table: "OrderedPrice",
                type: "numeric(18,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AddColumn<decimal>(
                name: "ConvertedRetailPrice",
                schema: "booking",
                table: "OrderedPrice",
                type: "numeric(18,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ExchangeRate",
                schema: "booking",
                table: "OrderedPrice",
                type: "numeric(18,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "RetailPrice",
                schema: "booking",
                table: "CartPrice",
                type: "numeric(18,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AddColumn<decimal>(
                name: "ConvertedRetailPrice",
                schema: "booking",
                table: "CartPrice",
                type: "numeric(18,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ExchangeRate",
                schema: "booking",
                table: "CartPrice",
                type: "numeric(18,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<int>(
                name: "CurrencyCode",
                schema: "booking",
                table: "Cart",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateTable(
                name: "CurrencyRate",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    ModifiedBy = table.Column<int>(type: "integer", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    BaseCurrency = table.Column<int>(type: "integer", nullable: false),
                    TargetCurrency = table.Column<int>(type: "integer", nullable: false),
                    Rate = table.Column<decimal>(type: "numeric(18,8)", precision: 18, scale: 8, nullable: false),
                    Provider = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    FetchedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyRate", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyRate_BaseCurrency_TargetCurrency",
                schema: "public",
                table: "CurrencyRate",
                columns: new[] { "BaseCurrency", "TargetCurrency" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CurrencyRate",
                schema: "public");

            migrationBuilder.DropColumn(
                name: "ConvertedRetailPrice",
                schema: "booking",
                table: "OrderedPrice");

            migrationBuilder.DropColumn(
                name: "ExchangeRate",
                schema: "booking",
                table: "OrderedPrice");

            migrationBuilder.DropColumn(
                name: "ConvertedRetailPrice",
                schema: "booking",
                table: "CartPrice");

            migrationBuilder.DropColumn(
                name: "ExchangeRate",
                schema: "booking",
                table: "CartPrice");

            migrationBuilder.AddColumn<int>(
                name: "CurrencyCode",
                schema: "booking",
                table: "OrderItem",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<decimal>(
                name: "RetailPrice",
                schema: "booking",
                table: "OrderedPrice",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "RetailPrice",
                schema: "booking",
                table: "CartPrice",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,4)");

            migrationBuilder.AlterColumn<int>(
                name: "CurrencyCode",
                schema: "booking",
                table: "Cart",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }
    }
}
