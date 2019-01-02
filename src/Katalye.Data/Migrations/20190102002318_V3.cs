using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Katalye.Data.Migrations
{
    public partial class V3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UnknownEvent",
                columns: table => new
                {
                    Id = table.Column<Guid>(),
                    Tag = table.Column<string>(),
                    Data = table.Column<string>(),
                    CreatedOn = table.Column<DateTimeOffset>(),
                    ModifiedOn = table.Column<DateTimeOffset>()
                },
                constraints: table => { table.PrimaryKey("PK_UnknownEvent", x => x.Id); });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UnknownEvent");
        }
    }
}