using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryManagement5th.Migrations
{
    /// <inheritdoc />
    public partial class AddedVatToBack : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Rate",
                table: "VATs",
                type: "decimal(5,2)",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmount",
                table: "ARInvoices",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "VatId",
                table: "ARInvoiceLines",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ARInvoiceLines_VatId",
                table: "ARInvoiceLines",
                column: "VatId");

            migrationBuilder.AddForeignKey(
                name: "FK_ARInvoiceLines_VATs_VatId",
                table: "ARInvoiceLines",
                column: "VatId",
                principalTable: "VATs",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ARInvoiceLines_VATs_VatId",
                table: "ARInvoiceLines");

            migrationBuilder.DropIndex(
                name: "IX_ARInvoiceLines_VatId",
                table: "ARInvoiceLines");

            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "ARInvoices");

            migrationBuilder.DropColumn(
                name: "VatId",
                table: "ARInvoiceLines");

            migrationBuilder.AlterColumn<float>(
                name: "Rate",
                table: "VATs",
                type: "real",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)");
        }
    }
}
