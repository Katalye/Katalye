using System;
using Microsoft.EntityFrameworkCore.Migrations;

// ReSharper disable All

namespace Katalye.Data.Migrations
{
    public partial class V4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ServerSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Key = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: false),
                    Version = table.Column<int>(nullable: false),
                    LastUpdated = table.Column<DateTimeOffset>(nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: false),
                    ModifiedOn = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_ServerSettings", x => x.Id); });

            migrationBuilder.CreateIndex(
                name: "IX_ServerSettings_Key",
                table: "ServerSettings",
                column: "Key",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServerSettings");
        }
    }
}