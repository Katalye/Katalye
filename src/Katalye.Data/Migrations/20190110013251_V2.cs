using Microsoft.EntityFrameworkCore.Migrations;

namespace Katalye.Data.Migrations
{
    public partial class V2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ChangedCount",
                table: "MinionReturnEvents",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FailedCount",
                table: "MinionReturnEvents",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SuccessCount",
                table: "MinionReturnEvents",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChangedCount",
                table: "MinionReturnEvents");

            migrationBuilder.DropColumn(
                name: "FailedCount",
                table: "MinionReturnEvents");

            migrationBuilder.DropColumn(
                name: "SuccessCount",
                table: "MinionReturnEvents");
        }
    }
}
