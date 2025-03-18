using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnterpriseManagementApp.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRentChangeModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "NewRate",
                table: "RentChanges",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ProcessedDate",
                table: "RentChanges",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "RentChanges",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "RentChanges",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "SubmittedDate",
                table: "RentChanges",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "RentChanges",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_RentChanges_UserId",
                table: "RentChanges",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_RentChanges_AspNetUsers_UserId",
                table: "RentChanges",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RentChanges_AspNetUsers_UserId",
                table: "RentChanges");

            migrationBuilder.DropIndex(
                name: "IX_RentChanges_UserId",
                table: "RentChanges");

            migrationBuilder.DropColumn(
                name: "NewRate",
                table: "RentChanges");

            migrationBuilder.DropColumn(
                name: "ProcessedDate",
                table: "RentChanges");

            migrationBuilder.DropColumn(
                name: "Reason",
                table: "RentChanges");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "RentChanges");

            migrationBuilder.DropColumn(
                name: "SubmittedDate",
                table: "RentChanges");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "RentChanges");
        }
    }
}
