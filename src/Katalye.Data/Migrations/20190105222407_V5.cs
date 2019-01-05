using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
// ReSharper disable All

namespace Katalye.Data.Migrations
{
    public partial class V5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "GrainGeneration",
                table: "Minions",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastGrainRefresh",
                table: "Minions",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MinionGrain",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    MinionId = table.Column<Guid>(nullable: false),
                    Path = table.Column<string>(nullable: false),
                    Values = table.Column<List<string>>(nullable: false),
                    Generation = table.Column<Guid>(nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: false),
                    ModifiedOn = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MinionGrain", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MinionGrain_Minions_MinionId",
                        column: x => x.MinionId,
                        principalTable: "Minions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Minions_GrainGeneration",
                table: "Minions",
                column: "GrainGeneration",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MinionGrain_MinionId_Generation_Path",
                table: "MinionGrain",
                columns: new[] { "MinionId", "Generation", "Path" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MinionGrain");

            migrationBuilder.DropIndex(
                name: "IX_Minions_GrainGeneration",
                table: "Minions");

            migrationBuilder.DropColumn(
                name: "GrainGeneration",
                table: "Minions");

            migrationBuilder.DropColumn(
                name: "LastGrainRefresh",
                table: "Minions");
        }
    }
}
