using System;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore.Migrations;
// ReSharper disable All

namespace Katalye.Data.Migrations
{
    public partial class V1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JobMinionEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Type = table.Column<string>(nullable: false),
                    Data = table.Column<string>(nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: false),
                    ModifiedOn = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_JobMinionEvents", x => x.Id); });

            migrationBuilder.CreateTable(
                name: "Jobs",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Jid = table.Column<string>(maxLength: 20, nullable: false),
                    Function = table.Column<string>(nullable: false),
                    TargetType = table.Column<string>(nullable: false),
                    Target = table.Column<List<string>>(nullable: false),
                    User = table.Column<string>(nullable: false),
                    Arguments = table.Column<string>(nullable: false),
                    MissingMinions = table.Column<List<string>>(nullable: true),
                    TimeStamp = table.Column<DateTimeOffset>(nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: false),
                    ModifiedOn = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_Jobs", x => x.Id); });

            migrationBuilder.CreateTable(
                name: "JobMinions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    JobId = table.Column<Guid>(nullable: false),
                    MinionId = table.Column<string>(nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: false),
                    ModifiedOn = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobMinions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobMinions_Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobMinions_JobId_MinionId",
                table: "JobMinions",
                columns: new[] {"JobId", "MinionId"},
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_Jid",
                table: "Jobs",
                column: "Jid",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobMinionEvents");

            migrationBuilder.DropTable(
                name: "JobMinions");

            migrationBuilder.DropTable(
                name: "Jobs");
        }
    }
}