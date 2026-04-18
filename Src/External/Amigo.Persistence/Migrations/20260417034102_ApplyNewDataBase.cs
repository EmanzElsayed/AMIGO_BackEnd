using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Amigo.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ApplyNewDataBase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "auth");

            migrationBuilder.EnsureSchema(
                name: "tour");

            migrationBuilder.EnsureSchema(
                name: "booking");

            migrationBuilder.EnsureSchema(
                name: "translation");

            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.EnsureSchema(
                name: "payment");

            migrationBuilder.EnsureSchema(
                name: "review");

            migrationBuilder.CreateTable(
                name: "Currency",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    ModifiedBy = table.Column<int>(type: "integer", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CurrencyCode = table.Column<int>(type: "integer", nullable: false),
                    Icon = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currency", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Destination",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    ModifiedBy = table.Column<int>(type: "integer", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CountryCode = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ImagePublicId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Destination", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    FullName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    Image = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Gender = table.Column<int>(type: "integer", nullable: true),
                    BirthDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Nationality = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Language = table.Column<int>(type: "integer", nullable: true),
                    LastLoginAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    ModifiedBy = table.Column<int>(type: "integer", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CurrencyTranslation",
                schema: "translation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    ModifiedBy = table.Column<int>(type: "integer", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CurrencyId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Language = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyTranslation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CurrencyTranslation_Currency_CurrencyId",
                        column: x => x.CurrencyId,
                        principalSchema: "public",
                        principalTable: "Currency",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DestinationTranslation",
                schema: "translation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    ModifiedBy = table.Column<int>(type: "integer", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DestinationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Language = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DestinationTranslation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DestinationTranslation_Destination_DestinationId",
                        column: x => x.DestinationId,
                        principalSchema: "public",
                        principalTable: "Destination",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tour",
                schema: "tour",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    ModifiedBy = table.Column<int>(type: "integer", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CurrencyCode = table.Column<int>(type: "integer", nullable: false),
                    Duration = table.Column<TimeSpan>(type: "interval", nullable: false),
                    Discount = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    GuideLanguage = table.Column<int>(type: "integer", nullable: true),
                    MeetingPoint = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsPitsAllowed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsWheelchairAvailable = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    UserType = table.Column<int>(type: "integer", nullable: false),
                    DestinationId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tour", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tour_Destination_DestinationId",
                        column: x => x.DestinationId,
                        principalSchema: "public",
                        principalTable: "Destination",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Address",
                schema: "auth",
                columns: table => new
                {
                    ApplicationUserId = table.Column<string>(type: "text", nullable: false),
                    BuildingNumber = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.ApplicationUserId);
                    table.ForeignKey(
                        name: "FK_Address_Users_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cart",
                schema: "booking",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    ModifiedBy = table.Column<int>(type: "integer", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    UserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    CartToken = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CurrencyCode = table.Column<int>(type: "integer", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cart", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cart_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Order",
                schema: "booking",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    ModifiedBy = table.Column<int>(type: "integer", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    UserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    Currency = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Order_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Token = table.Column<string>(type: "text", nullable: true),
                    RefreshToken = table.Column<string>(type: "text", nullable: true),
                    JwtId = table.Column<string>(type: "text", nullable: true),
                    IsUsed = table.Column<bool>(type: "boolean", nullable: false),
                    IsRevoked = table.Column<bool>(type: "boolean", nullable: false),
                    AddedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cancellation",
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
                    CancelationPolicyType = table.Column<int>(type: "integer", nullable: false),
                    CancellationBefore = table.Column<TimeSpan>(type: "interval", nullable: false),
                    RefundPercentage = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cancellation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cancellation_Tour_TourId",
                        column: x => x.TourId,
                        principalSchema: "tour",
                        principalTable: "Tour",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Favorites",
                schema: "auth",
                columns: table => new
                {
                    TourId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    ModifiedBy = table.Column<int>(type: "integer", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Favorites", x => new { x.TourId, x.UserId });
                    table.ForeignKey(
                        name: "FK_Favorites_Tour_TourId",
                        column: x => x.TourId,
                        principalSchema: "tour",
                        principalTable: "Tour",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Favorites_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Price",
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
                    Cost = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    UserType = table.Column<int>(type: "integer", nullable: false),
                    Discount = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Price", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Price_Tour_TourId",
                        column: x => x.TourId,
                        principalSchema: "tour",
                        principalTable: "Tour",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Review",
                schema: "review",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    ModifiedBy = table.Column<int>(type: "integer", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    TourId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Rate = table.Column<decimal>(type: "numeric(3,2)", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    TravelWith = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Review", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Review_Tour_TourId",
                        column: x => x.TourId,
                        principalSchema: "tour",
                        principalTable: "Tour",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Review_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TourImage",
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
                    ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ImagePublicId = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourImage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TourImage_Tour_TourId",
                        column: x => x.TourId,
                        principalSchema: "tour",
                        principalTable: "Tour",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TourInclusion",
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
                name: "TourSchedule",
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
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    AvailableDateStatus = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourSchedule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TourSchedule_Tour_TourId",
                        column: x => x.TourId,
                        principalSchema: "tour",
                        principalTable: "Tour",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TourTranslation",
                schema: "translation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    ModifiedBy = table.Column<int>(type: "integer", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    TourId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Language = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourTranslation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TourTranslation_Tour_TourId",
                        column: x => x.TourId,
                        principalSchema: "tour",
                        principalTable: "Tour",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItem",
                schema: "booking",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    ModifiedBy = table.Column<int>(type: "integer", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    TourId = table.Column<Guid>(type: "uuid", nullable: true),
                    SlotId = table.Column<Guid>(type: "uuid", nullable: true),
                    TourName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    DestinationName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    TourDate = table.Column<DateOnly>(type: "date", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    CurrencyCode = table.Column<int>(type: "integer", nullable: false),
                    Language = table.Column<int>(type: "integer", nullable: false),
                    MeetingPoint = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Duration = table.Column<TimeSpan>(type: "interval", nullable: false),
                    CancelationPolicyType = table.Column<int>(type: "integer", nullable: false),
                    CancellationBefore = table.Column<TimeSpan>(type: "interval", nullable: false),
                    RefundPercentage = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItem_Order_OrderId",
                        column: x => x.OrderId,
                        principalSchema: "booking",
                        principalTable: "Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payment",
                schema: "payment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    ModifiedBy = table.Column<int>(type: "integer", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PaymentMethod = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Note = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Currency = table.Column<int>(type: "integer", nullable: false),
                    PaidAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Provider = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ProviderTransactionId = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    RawResponseJson = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payment_Order_OrderId",
                        column: x => x.OrderId,
                        principalSchema: "booking",
                        principalTable: "Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CancellationTranslation",
                schema: "translation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    ModifiedBy = table.Column<int>(type: "integer", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CancellationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Language = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CancellationTranslation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CancellationTranslation_Cancellation_CancellationId",
                        column: x => x.CancellationId,
                        principalSchema: "tour",
                        principalTable: "Cancellation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PriceTranslation",
                schema: "translation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    ModifiedBy = table.Column<int>(type: "integer", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    PriceId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                    Language = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceTranslation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PriceTranslation_Price_PriceId",
                        column: x => x.PriceId,
                        principalSchema: "tour",
                        principalTable: "Price",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReviewImage",
                schema: "review",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    ModifiedBy = table.Column<int>(type: "integer", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    ReviewId = table.Column<Guid>(type: "uuid", nullable: false),
                    Image = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ReviewId1 = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewImage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReviewImage_Review_ReviewId",
                        column: x => x.ReviewId,
                        principalSchema: "review",
                        principalTable: "Review",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReviewImage_Review_ReviewId1",
                        column: x => x.ReviewId1,
                        principalSchema: "review",
                        principalTable: "Review",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ReviewTranslation",
                schema: "translation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    ModifiedBy = table.Column<int>(type: "integer", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    ReviewId = table.Column<Guid>(type: "uuid", nullable: false),
                    Comment = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Language = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewTranslation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReviewTranslation_Review_ReviewId",
                        column: x => x.ReviewId,
                        principalSchema: "review",
                        principalTable: "Review",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InclusionTranslation",
                schema: "translation",
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
                    table.PrimaryKey("PK_InclusionTranslation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InclusionTranslation_TourInclusion_TourInclusionId",
                        column: x => x.TourInclusionId,
                        principalSchema: "tour",
                        principalTable: "TourInclusion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AvailableSlots",
                schema: "tour",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    ModifiedBy = table.Column<int>(type: "integer", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    TourScheduleId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: true),
                    AvailableTimeStatus = table.Column<int>(type: "integer", nullable: false),
                    MaxCapacity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AvailableSlots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AvailableSlots_TourSchedule_TourScheduleId",
                        column: x => x.TourScheduleId,
                        principalSchema: "tour",
                        principalTable: "TourSchedule",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Booking",
                schema: "booking",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    ModifiedBy = table.Column<int>(type: "integer", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    OrderItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    CustomerName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    CustomerEmail = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    BookingNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ConfirmedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Booking", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Booking_OrderItem_OrderItemId",
                        column: x => x.OrderItemId,
                        principalSchema: "booking",
                        principalTable: "OrderItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Booking_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderedPrice",
                schema: "booking",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    ModifiedBy = table.Column<int>(type: "integer", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    OrderItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    RetailPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderedPrice", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderedPrice_OrderItem_OrderItemId",
                        column: x => x.OrderItemId,
                        principalSchema: "booking",
                        principalTable: "OrderItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Refund",
                schema: "booking",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    ModifiedBy = table.Column<int>(type: "integer", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    PaymentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ProviderRefundId = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    RequestedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RefundedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Refund", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Refund_Payment_PaymentId",
                        column: x => x.PaymentId,
                        principalSchema: "payment",
                        principalTable: "Payment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CartItem",
                schema: "booking",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    ModifiedBy = table.Column<int>(type: "integer", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CartId = table.Column<Guid>(type: "uuid", nullable: false),
                    TourId = table.Column<Guid>(type: "uuid", nullable: false),
                    SlotId = table.Column<Guid>(type: "uuid", nullable: false),
                    Language = table.Column<int>(type: "integer", nullable: false),
                    TourDate = table.Column<DateOnly>(type: "date", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    TourName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    DestinationName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartItem_AvailableSlots_SlotId",
                        column: x => x.SlotId,
                        principalSchema: "tour",
                        principalTable: "AvailableSlots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CartItem_Cart_CartId",
                        column: x => x.CartId,
                        principalSchema: "booking",
                        principalTable: "Cart",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CartItem_Tour_TourId",
                        column: x => x.TourId,
                        principalSchema: "tour",
                        principalTable: "Tour",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CancellationRequest",
                schema: "booking",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    ModifiedBy = table.Column<int>(type: "integer", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    BookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    Reason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    RefundAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    RequestedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CancellationRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CancellationRequest_Booking_BookingId",
                        column: x => x.BookingId,
                        principalSchema: "booking",
                        principalTable: "Booking",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Traveler",
                schema: "booking",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    ModifiedBy = table.Column<int>(type: "integer", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    BookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    FullName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Nationality = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    BirthDate = table.Column<DateOnly>(type: "date", nullable: true),
                    PassportNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PhoneNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Traveler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Traveler_Booking_BookingId",
                        column: x => x.BookingId,
                        principalSchema: "booking",
                        principalTable: "Booking",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Voucher",
                schema: "booking",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    ModifiedBy = table.Column<int>(type: "integer", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    BookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    VoucherNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IssuedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PdfUrl = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    QRCode = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    IsSentByEmail = table.Column<bool>(type: "boolean", nullable: false),
                    SentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Voucher", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Voucher_Booking_BookingId",
                        column: x => x.BookingId,
                        principalSchema: "booking",
                        principalTable: "Booking",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CartPrice",
                schema: "booking",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    ModifiedBy = table.Column<int>(type: "integer", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CartItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    RetailPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartPrice", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartPrice_CartItem_CartItemId",
                        column: x => x.CartItemId,
                        principalSchema: "booking",
                        principalTable: "CartItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AvailableSlots_StartTime_EndTime",
                schema: "tour",
                table: "AvailableSlots",
                columns: new[] { "StartTime", "EndTime" });

            migrationBuilder.CreateIndex(
                name: "IX_AvailableSlots_TourScheduleId",
                schema: "tour",
                table: "AvailableSlots",
                column: "TourScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_BookingNumber",
                schema: "booking",
                table: "Booking",
                column: "BookingNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_OrderItemId",
                schema: "booking",
                table: "Booking",
                column: "OrderItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Booking_UserId",
                schema: "booking",
                table: "Booking",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Cancellation_TourId",
                schema: "tour",
                table: "Cancellation",
                column: "TourId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CancellationRequest_BookingId",
                schema: "booking",
                table: "CancellationRequest",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_CancellationTranslation_CancellationId_Language",
                schema: "translation",
                table: "CancellationTranslation",
                columns: new[] { "CancellationId", "Language" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CancellationTranslation_Language",
                schema: "translation",
                table: "CancellationTranslation",
                column: "Language");

            migrationBuilder.CreateIndex(
                name: "IX_Cart_CartToken",
                schema: "booking",
                table: "Cart",
                column: "CartToken");

            migrationBuilder.CreateIndex(
                name: "IX_Cart_UserId",
                schema: "booking",
                table: "Cart",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItem_CartId",
                schema: "booking",
                table: "CartItem",
                column: "CartId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItem_SlotId",
                schema: "booking",
                table: "CartItem",
                column: "SlotId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItem_TourId",
                schema: "booking",
                table: "CartItem",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_CartPrice_CartItemId",
                schema: "booking",
                table: "CartPrice",
                column: "CartItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Currency_CurrencyCode",
                schema: "public",
                table: "Currency",
                column: "CurrencyCode");

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyTranslation_CurrencyId_Language",
                schema: "translation",
                table: "CurrencyTranslation",
                columns: new[] { "CurrencyId", "Language" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyTranslation_Language",
                schema: "translation",
                table: "CurrencyTranslation",
                column: "Language");

            migrationBuilder.CreateIndex(
                name: "IX_Destination_CountryCode",
                schema: "public",
                table: "Destination",
                column: "CountryCode");

            migrationBuilder.CreateIndex(
                name: "IX_Destination_IsActive",
                schema: "public",
                table: "Destination",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_DestinationTranslation_DestinationId_Language",
                schema: "translation",
                table: "DestinationTranslation",
                columns: new[] { "DestinationId", "Language" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DestinationTranslation_Language",
                schema: "translation",
                table: "DestinationTranslation",
                column: "Language");

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_UserId",
                schema: "auth",
                table: "Favorites",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_InclusionTranslation_Language",
                schema: "translation",
                table: "InclusionTranslation",
                column: "Language");

            migrationBuilder.CreateIndex(
                name: "IX_InclusionTranslation_TourInclusionId_Language",
                schema: "translation",
                table: "InclusionTranslation",
                columns: new[] { "TourInclusionId", "Language" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Order_Status",
                schema: "booking",
                table: "Order",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Order_UserId",
                schema: "booking",
                table: "Order",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderedPrice_OrderItemId",
                schema: "booking",
                table: "OrderedPrice",
                column: "OrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_OrderId",
                schema: "booking",
                table: "OrderItem",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_OrderId",
                schema: "payment",
                table: "Payment",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Price_TourId",
                schema: "tour",
                table: "Price",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_Price_TourId_UserType",
                schema: "tour",
                table: "Price",
                columns: new[] { "TourId", "UserType" });

            migrationBuilder.CreateIndex(
                name: "IX_PriceTranslation_Language",
                schema: "translation",
                table: "PriceTranslation",
                column: "Language");

            migrationBuilder.CreateIndex(
                name: "IX_PriceTranslation_PriceId_Language",
                schema: "translation",
                table: "PriceTranslation",
                columns: new[] { "PriceId", "Language" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Refund_PaymentId",
                schema: "booking",
                table: "Refund",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_Review_TourId",
                schema: "review",
                table: "Review",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_Review_UserId",
                schema: "review",
                table: "Review",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewImage_ReviewId",
                schema: "review",
                table: "ReviewImage",
                column: "ReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewImage_ReviewId1",
                schema: "review",
                table: "ReviewImage",
                column: "ReviewId1");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewTranslation_Language",
                schema: "translation",
                table: "ReviewTranslation",
                column: "Language");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewTranslation_ReviewId_Language",
                schema: "translation",
                table: "ReviewTranslation",
                columns: new[] { "ReviewId", "Language" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "Roles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tour_DestinationId",
                schema: "tour",
                table: "Tour",
                column: "DestinationId");

            migrationBuilder.CreateIndex(
                name: "IX_TourImage_TourId",
                schema: "tour",
                table: "TourImage",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_TourInclusion_TourId",
                schema: "tour",
                table: "TourInclusion",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_TourSchedule_StartDate_EndDate",
                schema: "tour",
                table: "TourSchedule",
                columns: new[] { "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_TourSchedule_TourId",
                schema: "tour",
                table: "TourSchedule",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_TourTranslation_TourId_Language",
                schema: "translation",
                table: "TourTranslation",
                columns: new[] { "TourId", "Language" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Traveler_BookingId",
                schema: "booking",
                table: "Traveler",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "Users",
                column: "NormalizedEmail",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_PhoneNumber",
                table: "Users",
                column: "PhoneNumber");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "Users",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Voucher_BookingId",
                schema: "booking",
                table: "Voucher",
                column: "BookingId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Voucher_VoucherNumber",
                schema: "booking",
                table: "Voucher",
                column: "VoucherNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Address",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "CancellationRequest",
                schema: "booking");

            migrationBuilder.DropTable(
                name: "CancellationTranslation",
                schema: "translation");

            migrationBuilder.DropTable(
                name: "CartPrice",
                schema: "booking");

            migrationBuilder.DropTable(
                name: "CurrencyTranslation",
                schema: "translation");

            migrationBuilder.DropTable(
                name: "DestinationTranslation",
                schema: "translation");

            migrationBuilder.DropTable(
                name: "Favorites",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "InclusionTranslation",
                schema: "translation");

            migrationBuilder.DropTable(
                name: "OrderedPrice",
                schema: "booking");

            migrationBuilder.DropTable(
                name: "PriceTranslation",
                schema: "translation");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "Refund",
                schema: "booking");

            migrationBuilder.DropTable(
                name: "ReviewImage",
                schema: "review");

            migrationBuilder.DropTable(
                name: "ReviewTranslation",
                schema: "translation");

            migrationBuilder.DropTable(
                name: "TourImage",
                schema: "tour");

            migrationBuilder.DropTable(
                name: "TourTranslation",
                schema: "translation");

            migrationBuilder.DropTable(
                name: "Traveler",
                schema: "booking");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "Voucher",
                schema: "booking");

            migrationBuilder.DropTable(
                name: "Cancellation",
                schema: "tour");

            migrationBuilder.DropTable(
                name: "CartItem",
                schema: "booking");

            migrationBuilder.DropTable(
                name: "Currency",
                schema: "public");

            migrationBuilder.DropTable(
                name: "TourInclusion",
                schema: "tour");

            migrationBuilder.DropTable(
                name: "Price",
                schema: "tour");

            migrationBuilder.DropTable(
                name: "Payment",
                schema: "payment");

            migrationBuilder.DropTable(
                name: "Review",
                schema: "review");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Booking",
                schema: "booking");

            migrationBuilder.DropTable(
                name: "AvailableSlots",
                schema: "tour");

            migrationBuilder.DropTable(
                name: "Cart",
                schema: "booking");

            migrationBuilder.DropTable(
                name: "OrderItem",
                schema: "booking");

            migrationBuilder.DropTable(
                name: "TourSchedule",
                schema: "tour");

            migrationBuilder.DropTable(
                name: "Order",
                schema: "booking");

            migrationBuilder.DropTable(
                name: "Tour",
                schema: "tour");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Destination",
                schema: "public");
        }
    }
}
