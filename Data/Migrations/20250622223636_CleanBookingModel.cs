using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace assignment_mvc_carrental.Data.Migrations
{
    /// <inheritdoc />
    public partial class CleanBookingModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Dessa är redan fixade manuellt i databasen:
            // - CustomerId har tagits bort
            // - ApplicationUserId har lagts till
            // - FK har skapats manuellt
            // Därför kommenterar vi ut dessa:

            // migrationBuilder.RenameColumn(
            //     name: "CustomerId",
            //     table: "BookingSet",
            //     newName: "ApplicationUserId");

            // migrationBuilder.RenameIndex(
            //     name: "IX_BookingSet_CustomerId",
            //     table: "BookingSet",
            //     newName: "IX_BookingSet_ApplicationUserId");

            // migrationBuilder.AddForeignKey(
            //     name: "FK_BookingSet_AspNetUsers_ApplicationUserId",
            //     table: "BookingSet",
            //     column: "ApplicationUserId",
            //     principalTable: "AspNetUsers",
            //     principalColumn: "Id",
            //     onDelete: ReferentialAction.Cascade);

            // Dessa kolumner finns kvar och tas bort som tänkt:
            migrationBuilder.DropColumn(
                name: "CustomerFirstName",
                table: "BookingSet");

            migrationBuilder.DropColumn(
                name: "CustomerLastName",
                table: "BookingSet");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Vi återskapar kolumnerna om vi rullar tillbaka:
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

            // Dessa läggs inte tillbaka automatiskt, eftersom du redan manuellt brutit bortkopplingen:
            // Vill du ha fullständig reversering, kan du åter lägga till kolumnen och FK så här:

            migrationBuilder.DropForeignKey(
                name: "FK_BookingSet_AspNetUsers_ApplicationUserId",
                table: "BookingSet");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "BookingSet");

            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "BookingSet",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_BookingSet_AspNetUsers_CustomerId",
                table: "BookingSet",
                column: "CustomerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
