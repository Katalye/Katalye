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
                name: "Jobs",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Jid = table.Column<string>(maxLength: 20, nullable: false),
                    Function = table.Column<string>(nullable: false),
                    Arguments = table.Column<string>(nullable: false),
                    Version = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: false),
                    ModifiedOn = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Minions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    MinionSlug = table.Column<string>(nullable: false),
                    LastAuthentication = table.Column<DateTimeOffset>(nullable: true),
                    LastSeen = table.Column<DateTimeOffset>(nullable: true),
                    LastGrainRefresh = table.Column<DateTimeOffset>(nullable: true),
                    GrainGeneration = table.Column<Guid>(nullable: true),
                    Version = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: false),
                    ModifiedOn = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Minions", x => x.Id);
                });

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
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnknownEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JobCreationEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    JobId = table.Column<Guid>(nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(nullable: false),
                    User = table.Column<string>(nullable: false),
                    Minions = table.Column<List<string>>(nullable: true),
                    MissingMinions = table.Column<List<string>>(nullable: true),
                    Targets = table.Column<List<string>>(nullable: true),
                    TargetType = table.Column<string>(nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: false),
                    ModifiedOn = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobCreationEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobCreationEvents_Jobs_JobId",
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
                    PublicKeyHash = table.Column<string>(nullable: false),
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
                name: "MinionGrains",
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
                    table.PrimaryKey("PK_MinionGrains", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MinionGrains_Minions_MinionId",
                        column: x => x.MinionId,
                        principalTable: "Minions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MinionReturnEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    MinionId = table.Column<Guid>(nullable: false),
                    JobId = table.Column<Guid>(nullable: false),
                    ReturnData = table.Column<string>(nullable: false),
                    Success = table.Column<bool>(nullable: false),
                    ReturnCode = table.Column<long>(nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: false),
                    ModifiedOn = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MinionReturnEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MinionReturnEvents_Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MinionReturnEvents_Minions_MinionId",
                        column: x => x.MinionId,
                        principalTable: "Minions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MinionGrainValues",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    MinionGrainId = table.Column<Guid>(nullable: false),
                    Value = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MinionGrainValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MinionGrainValues_MinionGrains_MinionGrainId",
                        column: x => x.MinionGrainId,
                        principalTable: "MinionGrains",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobCreationEvents_JobId",
                table: "JobCreationEvents",
                column: "JobId",
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
                name: "IX_MinionAuthenticationEvents_PublicKeyHash",
                table: "MinionAuthenticationEvents",
                column: "PublicKeyHash");

            migrationBuilder.CreateIndex(
                name: "IX_MinionGrains_MinionId_Generation_Path",
                table: "MinionGrains",
                columns: new[] { "MinionId", "Generation", "Path" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MinionGrainValues_MinionGrainId",
                table: "MinionGrainValues",
                column: "MinionGrainId");

            migrationBuilder.CreateIndex(
                name: "IX_MinionReturnEvents_JobId",
                table: "MinionReturnEvents",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_MinionReturnEvents_MinionId_JobId",
                table: "MinionReturnEvents",
                columns: new[] { "MinionId", "JobId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Minions_GrainGeneration",
                table: "Minions",
                column: "GrainGeneration",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Minions_MinionSlug",
                table: "Minions",
                column: "MinionSlug",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobCreationEvents");

            migrationBuilder.DropTable(
                name: "MinionAuthenticationEvents");

            migrationBuilder.DropTable(
                name: "MinionGrainValues");

            migrationBuilder.DropTable(
                name: "MinionReturnEvents");

            migrationBuilder.DropTable(
                name: "UnknownEvents");

            migrationBuilder.DropTable(
                name: "MinionGrains");

            migrationBuilder.DropTable(
                name: "Jobs");

            migrationBuilder.DropTable(
                name: "Minions");
        }
    }
}
