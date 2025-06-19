using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace assignment_mvc_carrental.Data.Migrations
{
    /// <inheritdoc />
    public partial class ForceColumnRefresh : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DummyTriggerColumn",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DummyTriggerColumn",
                table: "AspNetUsers");
        }
    }
}
