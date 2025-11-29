using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryManagement5th.Migrations
{
    /// <inheritdoc />
    public partial class fixedAPInvoice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmount",
                table: "APInvoices",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "VatId",
                table: "APInvoiceLines",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_APInvoiceLines_VatId",
                table: "APInvoiceLines",
                column: "VatId");

            migrationBuilder.AddForeignKey(
                name: "FK_APInvoiceLines_VATs_VatId",
                table: "APInvoiceLines",
                column: "VatId",
                principalTable: "VATs",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_APInvoiceLines_VATs_VatId",
                table: "APInvoiceLines");

            migrationBuilder.DropIndex(
                name: "IX_APInvoiceLines_VatId",
                table: "APInvoiceLines");

            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "APInvoices");

            migrationBuilder.DropColumn(
                name: "VatId",
                table: "APInvoiceLines");
        }
    }
}
