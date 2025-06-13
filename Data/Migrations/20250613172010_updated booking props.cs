using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace assignment_mvc_carrental.Data.Migrations
{
    /// <inheritdoc />
    public partial class updatedbookingprops : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomerEmail",
                table: "BookingSet",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CustomerFirstName",
                table: "BookingSet",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CustomerLastName",
                table: "BookingSet",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerEmail",
                table: "BookingSet");

            migrationBuilder.DropColumn(
                name: "CustomerFirstName",
                table: "BookingSet");

            migrationBuilder.DropColumn(
                name: "CustomerLastName",
                table: "BookingSet");
        }
    }
}
