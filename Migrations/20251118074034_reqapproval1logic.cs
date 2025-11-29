using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryManagement5th.Migrations
{
    /// <inheritdoc />
    public partial class reqapproval1logic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RejectionsCount",
                table: "PendingApprovalRequests",
                newName: "RequestedById");

            migrationBuilder.RenameColumn(
                name: "CreatedByUserId",
                table: "PendingApprovalRequests",
                newName: "RejectedCount");

            migrationBuilder.RenameColumn(
                name: "ApprovalsCount",
                table: "PendingApprovalRequests",
                newName: "ApprovedCount");

            migrationBuilder.AlterColumn<int>(
                name: "InvoiceType",
                table: "PendingApprovalRequests",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "PendingApprovalRequests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "PendingApprovalRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "PendingApprovalRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "PendingApprovalRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedBy",
                table: "PendingApprovalRequests",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ApprovalVotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PendingApprovalRequestId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalVotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApprovalVotes_PendingApprovalRequests_PendingApprovalRequestId",
                        column: x => x.PendingApprovalRequestId,
                        principalTable: "PendingApprovalRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalVotes_PendingApprovalRequestId",
                table: "ApprovalVotes",
                column: "PendingApprovalRequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApprovalVotes");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "PendingApprovalRequests");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "PendingApprovalRequests");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "PendingApprovalRequests");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "PendingApprovalRequests");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "PendingApprovalRequests");

            migrationBuilder.RenameColumn(
                name: "RequestedById",
                table: "PendingApprovalRequests",
                newName: "RejectionsCount");

            migrationBuilder.RenameColumn(
                name: "RejectedCount",
                table: "PendingApprovalRequests",
                newName: "CreatedByUserId");

            migrationBuilder.RenameColumn(
                name: "ApprovedCount",
                table: "PendingApprovalRequests",
                newName: "ApprovalsCount");

            migrationBuilder.AlterColumn<string>(
                name: "InvoiceType",
                table: "PendingApprovalRequests",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
