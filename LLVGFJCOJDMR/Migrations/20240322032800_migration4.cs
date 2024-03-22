using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LLVGFJCOJDMR.Migrations
{
    public partial class migration4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Rols",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[] { 1, "soy admin", "Administrador" });

            migrationBuilder.InsertData(
                table: "Rols",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[] { 2, "soy empleado", "Empleado" });

            migrationBuilder.InsertData(
                table: "Rols",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[] { 3, "soy gerente", "Gerente" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "Image", "Password", "RolId", "Status", "UserName" },
                values: new object[] { 1, "root@gmail.com", null, "be4c75f3853a9f55a0b27fbaeb1a0dde", 1, 1, "Root" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Rols",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Rols",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Rols",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
