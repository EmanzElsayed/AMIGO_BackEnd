using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Amigo.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class fixTranslationRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TourTranslation_Tour_TourId1",
                schema: "tour",
                table: "TourTranslation");

            migrationBuilder.DropIndex(
                name: "IX_TourTranslation_TourId1",
                schema: "tour",
                table: "TourTranslation");

            migrationBuilder.DropColumn(
                name: "TourId1",
                schema: "tour",
                table: "TourTranslation");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TourId1",
                schema: "tour",
                table: "TourTranslation",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TourTranslation_TourId1",
                schema: "tour",
                table: "TourTranslation",
                column: "TourId1");

            migrationBuilder.AddForeignKey(
                name: "FK_TourTranslation_Tour_TourId1",
                schema: "tour",
                table: "TourTranslation",
                column: "TourId1",
                principalSchema: "tour",
                principalTable: "Tour",
                principalColumn: "Id");
        }
    }
}
