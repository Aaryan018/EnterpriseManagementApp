using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnterpriseManagementApp.Migrations
{
    /// <inheritdoc />
    public partial class ConfigureRenter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmergencyContactName",
                table: "Renters");

            migrationBuilder.DropColumn(
                name: "EmergencyContactPhone",
                table: "Renters");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "Renters",
                newName: "ContactNumber");

            migrationBuilder.RenameColumn(
                name: "FirstName",
                table: "Renters",
                newName: "Address");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ContactNumber",
                table: "Renters",
                newName: "LastName");

            migrationBuilder.RenameColumn(
                name: "Address",
                table: "Renters",
                newName: "FirstName");

            migrationBuilder.AddColumn<string>(
                name: "EmergencyContactName",
                table: "Renters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmergencyContactPhone",
                table: "Renters",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
