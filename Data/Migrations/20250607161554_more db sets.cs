using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace assignment_mvc_carrental.Data.Migrations
{
    /// <inheritdoc />
    public partial class moredbsets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerSet_UserRole_UserRoleId",
                table: "CustomerSet");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRole",
                table: "UserRole");

            migrationBuilder.RenameTable(
                name: "UserRole",
                newName: "UserRoleSet");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRoleSet",
                table: "UserRoleSet",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "AdminSet",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserRoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminSet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdminSet_UserRoleSet_UserRoleId",
                        column: x => x.UserRoleId,
                        principalTable: "UserRoleSet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdminSet_UserRoleId",
                table: "AdminSet",
                column: "UserRoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerSet_UserRoleSet_UserRoleId",
                table: "CustomerSet",
                column: "UserRoleId",
                principalTable: "UserRoleSet",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerSet_UserRoleSet_UserRoleId",
                table: "CustomerSet");

            migrationBuilder.DropTable(
                name: "AdminSet");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRoleSet",
                table: "UserRoleSet");

            migrationBuilder.RenameTable(
                name: "UserRoleSet",
                newName: "UserRole");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRole",
                table: "UserRole",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerSet_UserRole_UserRoleId",
                table: "CustomerSet",
                column: "UserRoleId",
                principalTable: "UserRole",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
