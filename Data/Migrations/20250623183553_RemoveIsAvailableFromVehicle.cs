using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace assignment_mvc_carrental.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIsAvailableFromVehicle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "VehicleSet");

            //migrationBuilder.DropColumn(
            //    name: "VehicleTitle",
            //    table: "BookingSet");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "VehicleSet",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "VehicleTitle",
                table: "BookingSet",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
