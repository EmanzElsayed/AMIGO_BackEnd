using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Amigo.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class changeRelationOfBookingAndOrderItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Booking_OrderItem_OrderItemId",
                schema: "booking",
                table: "Booking");

           

         


            


          



            migrationBuilder.AddForeignKey(
                name: "FK_Booking_OrderItem_OrderItemId",
                schema: "booking",
                table: "Booking",
                column: "OrderItemId",
                principalSchema: "booking",
                principalTable: "OrderItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Booking_OrderItem_OrderItemId",
                schema: "booking",
                table: "Booking");

           


         
      




            migrationBuilder.AddForeignKey(
                name: "FK_Booking_OrderItem_OrderItemId",
                schema: "booking",
                table: "Booking",
                column: "OrderItemId",
                principalSchema: "booking",
                principalTable: "OrderItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
