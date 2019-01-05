using Microsoft.EntityFrameworkCore.Migrations;

namespace Katalye.Data.Migrations
{
    public partial class V4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PublicKeyHash",
                table: "MinionAuthenticationEvents",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_MinionAuthenticationEvents_PublicKeyHash",
                table: "MinionAuthenticationEvents",
                column: "PublicKeyHash");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MinionAuthenticationEvents_PublicKeyHash",
                table: "MinionAuthenticationEvents");

            migrationBuilder.DropColumn(
                name: "PublicKeyHash",
                table: "MinionAuthenticationEvents");
        }
    }
}
