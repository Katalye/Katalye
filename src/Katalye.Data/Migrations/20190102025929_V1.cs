using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Katalye.Data.Migrations
{
    [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
    public partial class V1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "Minions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    MinionSlug = table.Column<string>(nullable: false),
                    LastAuthentication = table.Column<DateTimeOffset>(nullable: true),
                    Version = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: false),
                    ModifiedOn = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_Minions", x => x.Id); });

            migrationBuilder.CreateTable(
                name: "UnknownEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Tag = table.Column<string>(nullable: false),
                    Data = table.Column<string>(nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: false),
                    ModifiedOn = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_UnknownEvents", x => x.Id); });

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

            migrationBuilder.CreateTable(
                name: "MinionAuthenticationEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    MinionId = table.Column<Guid>(nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(nullable: false),
                    Action = table.Column<string>(nullable: false),
                    PublicKey = table.Column<string>(nullable: false),
                    Success = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: false),
                    ModifiedOn = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MinionAuthenticationEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MinionAuthenticationEvents_Minions_MinionId",
                        column: x => x.MinionId,
                        principalTable: "Minions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobMinionEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    JobMinionId = table.Column<Guid>(nullable: false),
                    ReturnData = table.Column<string>(nullable: false),
                    Success = table.Column<bool>(nullable: false),
                    ReturnCode = table.Column<long>(nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: false),
                    ModifiedOn = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobMinionEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobMinionEvents_JobMinions_JobMinionId",
                        column: x => x.JobMinionId,
                        principalTable: "JobMinions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobMinionEvents_JobMinionId",
                table: "JobMinionEvents",
                column: "JobMinionId");

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

            migrationBuilder.CreateIndex(
                name: "IX_MinionAuthenticationEvents_MinionId",
                table: "MinionAuthenticationEvents",
                column: "MinionId");

            migrationBuilder.CreateIndex(
                name: "IX_Minions_MinionSlug",
                table: "Minions",
                column: "MinionSlug",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobMinionEvents");

            migrationBuilder.DropTable(
                name: "MinionAuthenticationEvents");

            migrationBuilder.DropTable(
                name: "UnknownEvents");

            migrationBuilder.DropTable(
                name: "JobMinions");

            migrationBuilder.DropTable(
                name: "Minions");

            migrationBuilder.DropTable(
                name: "Jobs");
        }
    }
}