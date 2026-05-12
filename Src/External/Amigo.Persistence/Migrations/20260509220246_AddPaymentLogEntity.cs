using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Amigo.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentLogEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.DropIndex(
                name: "IX_WebhookEventLog_ProviderEventId",
                schema: "booking",
                table: "WebhookEventLog");

           

           
           


         

            migrationBuilder.CreateTable(
                name: "PaymentRequestLog",
                schema: "payment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    ModifiedBy = table.Column<int>(type: "integer", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "TIMEZONE('UTC', NOW())"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    RequestId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProviderPaymentId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ResponseJson = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentRequestLog", x => x.Id);
                });

          
            migrationBuilder.CreateIndex(
                name: "IX_WebhookEventLog_Provider_ProviderEventId",
                schema: "booking",
                table: "WebhookEventLog",
                columns: new[] { "Provider", "ProviderEventId" },
                unique: true);

          

            migrationBuilder.CreateIndex(
                name: "IX_PaymentRequestLog_OrderId",
                schema: "payment",
                table: "PaymentRequestLog",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentRequestLog_ProviderPaymentId",
                schema: "payment",
                table: "PaymentRequestLog",
                column: "ProviderPaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentRequestLog_RequestId",
                schema: "payment",
                table: "PaymentRequestLog",
                column: "RequestId",
                unique: true);


           
         
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.DropTable(
                name: "PaymentRequestLog",
                schema: "payment");

           

            migrationBuilder.DropIndex(
                name: "IX_WebhookEventLog_Provider_ProviderEventId",
                schema: "booking",
                table: "WebhookEventLog");

          

         



          

            migrationBuilder.CreateIndex(
                name: "IX_WebhookEventLog_ProviderEventId",
                schema: "booking",
                table: "WebhookEventLog",
                column: "ProviderEventId",
                unique: true);

        

         
        }
    }
}
