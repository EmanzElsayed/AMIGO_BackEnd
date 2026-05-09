using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Amigo.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ApplyContryInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Destination_CountryCode",
                schema: "public",
                table: "Destination");

            migrationBuilder.DropColumn(
                name: "CountryCode",
                schema: "public",
                table: "Destination");

         

            migrationBuilder.AddColumn<Guid>(
                name: "CountryInfoId",
                schema: "public",
                table: "Destination",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Destination_CountryInfoId",
                schema: "public",
                table: "Destination",
                column: "CountryInfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Destination_CountryInfo_CountryInfoId",
                schema: "public",
                table: "Destination",
                column: "CountryInfoId",
                principalSchema: "public",
                principalTable: "CountryInfo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Destination_CountryInfo_CountryInfoId",
                schema: "public",
                table: "Destination");

            migrationBuilder.DropIndex(
                name: "IX_Destination_CountryInfoId",
                schema: "public",
                table: "Destination");

            migrationBuilder.DropColumn(
                name: "CountryInfoId",
                schema: "public",
                table: "Destination");

         

         

            migrationBuilder.AddColumn<int>(
                name: "CountryCode",
                schema: "public",
                table: "Destination",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Destination_CountryCode",
                schema: "public",
                table: "Destination",
                column: "CountryCode");
        }
    }
}
