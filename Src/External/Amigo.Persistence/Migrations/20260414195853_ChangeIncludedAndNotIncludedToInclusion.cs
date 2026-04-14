using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Amigo.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChangeIncludedAndNotIncludedToInclusion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TourIncluded",
                schema: "tour");

            migrationBuilder.DropTable(
                name: "TourNotIncluded",
                schema: "tour");

            migrationBuilder.CreateTable(
                name: "TourInclusion",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    ModifiedBy = table.Column<int>(type: "integer", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    TourId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsIncluded = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourInclusion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TourInclusion_Tour_TourId",
                        column: x => x.TourId,
                        principalSchema: "tour",
                        principalTable: "Tour",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InclusionTranslations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    ModifiedBy = table.Column<int>(type: "integer", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    TourInclusionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Text = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                    Language = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InclusionTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InclusionTranslations_TourInclusion_TourInclusionId",
                        column: x => x.TourInclusionId,
                        principalTable: "TourInclusion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InclusionTranslations_Language",
                table: "InclusionTranslations",
                column: "Language");

            migrationBuilder.CreateIndex(
                name: "IX_InclusionTranslations_TourInclusionId_Language",
                table: "InclusionTranslations",
                columns: new[] { "TourInclusionId", "Language" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TourInclusion_TourId",
                table: "TourInclusion",
                column: "TourId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InclusionTranslations");

            migrationBuilder.DropTable(
                name: "TourInclusion");

            migrationBuilder.CreateTable(
                name: "TourIncluded",
                schema: "tour",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    ModifiedBy = table.Column<int>(type: "integer", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    TourId = table.Column<Guid>(type: "uuid", nullable: false),
                    Included = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Language = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourIncluded", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TourIncluded_Tour_TourId",
                        column: x => x.TourId,
                        principalSchema: "tour",
                        principalTable: "Tour",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TourNotIncluded",
                schema: "tour",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    ModifiedBy = table.Column<int>(type: "integer", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    TourId = table.Column<Guid>(type: "uuid", nullable: false),
                    Language = table.Column<int>(type: "integer", nullable: false),
                    NotIncluded = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourNotIncluded", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TourNotIncluded_Tour_TourId",
                        column: x => x.TourId,
                        principalSchema: "tour",
                        principalTable: "Tour",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TourIncluded_TourId",
                schema: "tour",
                table: "TourIncluded",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_TourIncluded_TourId_Language",
                schema: "tour",
                table: "TourIncluded",
                columns: new[] { "TourId", "Language" });

            migrationBuilder.CreateIndex(
                name: "IX_TourNotIncluded_TourId",
                schema: "tour",
                table: "TourNotIncluded",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_TourNotIncluded_TourId_Language",
                schema: "tour",
                table: "TourNotIncluded",
                columns: new[] { "TourId", "Language" });
        }
    }
}
