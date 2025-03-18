using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnterpriseManagementApp.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRentChangeRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RentChanges_AspNetUsers_UserId",
                table: "RentChanges");

            migrationBuilder.DropForeignKey(
                name: "FK_RentChanges_Assets_AssetId",
                table: "RentChanges");

            migrationBuilder.AddForeignKey(
                name: "FK_RentChanges_AspNetUsers_UserId",
                table: "RentChanges",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RentChanges_Assets_AssetId",
                table: "RentChanges",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "AssetId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RentChanges_AspNetUsers_UserId",
                table: "RentChanges");

            migrationBuilder.DropForeignKey(
                name: "FK_RentChanges_Assets_AssetId",
                table: "RentChanges");

            migrationBuilder.AddForeignKey(
                name: "FK_RentChanges_AspNetUsers_UserId",
                table: "RentChanges",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RentChanges_Assets_AssetId",
                table: "RentChanges",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "AssetId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
