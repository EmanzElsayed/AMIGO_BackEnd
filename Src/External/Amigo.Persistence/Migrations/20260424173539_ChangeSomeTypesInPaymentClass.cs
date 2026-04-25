using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Amigo.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChangeSomeTypesInPaymentClass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            ALTER TABLE payment.""Payment""
            ALTER COLUMN ""Provider"" TYPE integer
            USING CASE
                WHEN ""Provider"" = 'Paypal' THEN 0
                WHEN ""Provider"" = 'Stripe' THEN 1
                
                ELSE NULL
            END;
        ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
        ALTER TABLE payment.""Payment""
        ALTER COLUMN ""Provider"" TYPE varchar(100)
        USING CASE
            WHEN ""Provider"" = 0 THEN 'Paypal'
            WHEN ""Provider"" = 1 THEN 'Stripe'
               
            ELSE NULL
        END;
    ");
        }
    }
}
