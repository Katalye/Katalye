using Microsoft.EntityFrameworkCore.Migrations;

namespace Katalye.Data.Migrations
{
    public partial class V6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MinionGrain_Minions_MinionId",
                table: "MinionGrain");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MinionGrain",
                table: "MinionGrain");

            migrationBuilder.RenameTable(
                name: "MinionGrain",
                newName: "MinionGrains");

            migrationBuilder.RenameIndex(
                name: "IX_MinionGrain_MinionId_Generation_Path",
                table: "MinionGrains",
                newName: "IX_MinionGrains_MinionId_Generation_Path");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MinionGrains",
                table: "MinionGrains",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MinionGrains_Minions_MinionId",
                table: "MinionGrains",
                column: "MinionId",
                principalTable: "Minions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MinionGrains_Minions_MinionId",
                table: "MinionGrains");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MinionGrains",
                table: "MinionGrains");

            migrationBuilder.RenameTable(
                name: "MinionGrains",
                newName: "MinionGrain");

            migrationBuilder.RenameIndex(
                name: "IX_MinionGrains_MinionId_Generation_Path",
                table: "MinionGrain",
                newName: "IX_MinionGrain_MinionId_Generation_Path");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MinionGrain",
                table: "MinionGrain",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MinionGrain_Minions_MinionId",
                table: "MinionGrain",
                column: "MinionId",
                principalTable: "Minions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
