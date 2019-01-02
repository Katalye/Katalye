using Microsoft.EntityFrameworkCore.Migrations;

namespace Katalye.Data.Migrations
{
    public partial class V4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UnknownEvent",
                table: "UnknownEvent");

            migrationBuilder.RenameTable(
                name: "UnknownEvent",
                newName: "UnknownEvents");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UnknownEvents",
                table: "UnknownEvents",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UnknownEvents",
                table: "UnknownEvents");

            migrationBuilder.RenameTable(
                name: "UnknownEvents",
                newName: "UnknownEvent");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UnknownEvent",
                table: "UnknownEvent",
                column: "Id");
        }
    }
}
