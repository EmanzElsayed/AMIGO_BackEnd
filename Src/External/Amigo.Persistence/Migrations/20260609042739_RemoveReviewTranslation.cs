using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Amigo.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveReviewTranslation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReviewTranslation_Review_ReviewId1",
                schema: "translation",
                table: "ReviewTranslation");

            migrationBuilder.DropIndex(
                name: "IX_ReviewTranslation_ReviewId1",
                schema: "translation",
                table: "ReviewTranslation");

            migrationBuilder.DropColumn(
                name: "ReviewId1",
                schema: "translation",
                table: "ReviewTranslation");

            migrationBuilder.AddColumn<string>(
                name: "Comment",
                schema: "review",
                table: "Review",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comment",
                schema: "review",
                table: "Review");

            migrationBuilder.AddColumn<Guid>(
                name: "ReviewId1",
                schema: "translation",
                table: "ReviewTranslation",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReviewTranslation_ReviewId1",
                schema: "translation",
                table: "ReviewTranslation",
                column: "ReviewId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewTranslation_Review_ReviewId1",
                schema: "translation",
                table: "ReviewTranslation",
                column: "ReviewId1",
                principalSchema: "review",
                principalTable: "Review",
                principalColumn: "Id");
        }
    }
}
