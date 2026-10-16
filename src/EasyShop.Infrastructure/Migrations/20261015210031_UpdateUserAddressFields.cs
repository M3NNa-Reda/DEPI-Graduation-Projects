using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserAddressFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "State",
                table: "UserAddresses",
                newName: "Street");

            migrationBuilder.RenameColumn(
                name: "AddressLine",
                table: "UserAddresses",
                newName: "Area");

            migrationBuilder.AlterColumn<string>(
                name: "PostalCode",
                table: "UserAddresses",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "ApartmentNumber",
                table: "UserAddresses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BuildingNumber",
                table: "UserAddresses",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApartmentNumber",
                table: "UserAddresses");

            migrationBuilder.DropColumn(
                name: "BuildingNumber",
                table: "UserAddresses");

            migrationBuilder.RenameColumn(
                name: "Street",
                table: "UserAddresses",
                newName: "State");

            migrationBuilder.RenameColumn(
                name: "Area",
                table: "UserAddresses",
                newName: "AddressLine");

            migrationBuilder.AlterColumn<string>(
                name: "PostalCode",
                table: "UserAddresses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
