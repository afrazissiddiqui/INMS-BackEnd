using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryManagement5th.Migrations
{
    /// <inheritdoc />
    public partial class UPDATEDBPARTNER : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "BusinessPartners",
                newName: "FirstName");

            migrationBuilder.AddColumn<string>(
                name: "BusinessAddress",
                table: "BusinessPartners",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BusinessName",
                table: "BusinessPartners",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "BusinessPartners",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PhoneNumber",
                table: "BusinessPartners",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BusinessAddress",
                table: "BusinessPartners");

            migrationBuilder.DropColumn(
                name: "BusinessName",
                table: "BusinessPartners");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "BusinessPartners");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "BusinessPartners");

            migrationBuilder.RenameColumn(
                name: "FirstName",
                table: "BusinessPartners",
                newName: "Name");
        }
    }
}
