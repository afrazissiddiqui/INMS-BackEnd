using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryManagement5th.Migrations
{
    /// <inheritdoc />
    public partial class BuyNSalePrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Price",
                table: "Items",
                newName: "SalePrice");

            migrationBuilder.AddColumn<decimal>(
                name: "BuyPrice",
                table: "Items",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BuyPrice",
                table: "Items");

            migrationBuilder.RenameColumn(
                name: "SalePrice",
                table: "Items",
                newName: "Price");
        }
    }
}
