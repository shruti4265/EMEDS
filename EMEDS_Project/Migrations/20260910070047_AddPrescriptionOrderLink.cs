using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMEDS_Project.Migrations
{
    /// <inheritdoc />
    public partial class AddPrescriptionOrderLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderId",
                table: "Prescriptions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Prescriptions_OrderId",
                table: "Prescriptions",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_MedicineId",
                table: "OrderItems",
                column: "MedicineId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Medicines_MedicineId",
                table: "OrderItems",
                column: "MedicineId",
                principalTable: "Medicines",
                principalColumn: "MedicineId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Prescriptions_Orders_OrderId",
                table: "Prescriptions",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "OrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Medicines_MedicineId",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Prescriptions_Orders_OrderId",
                table: "Prescriptions");

            migrationBuilder.DropIndex(
                name: "IX_Prescriptions_OrderId",
                table: "Prescriptions");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_MedicineId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "Prescriptions");
        }
    }
}
