using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Katalye.Data.Migrations
{
    public partial class V2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Data",
                table: "JobMinionEvents");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "JobMinionEvents");

            migrationBuilder.AddColumn<Guid>(
                name: "JobMinionId",
                table: "JobMinionEvents",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<long>(
                name: "ReturnCode",
                table: "JobMinionEvents",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "ReturnData",
                table: "JobMinionEvents",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "Success",
                table: "JobMinionEvents",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Timestamp",
                table: "JobMinionEvents",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.CreateIndex(
                name: "IX_JobMinionEvents_JobMinionId",
                table: "JobMinionEvents",
                column: "JobMinionId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobMinionEvents_JobMinions_JobMinionId",
                table: "JobMinionEvents",
                column: "JobMinionId",
                principalTable: "JobMinions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobMinionEvents_JobMinions_JobMinionId",
                table: "JobMinionEvents");

            migrationBuilder.DropIndex(
                name: "IX_JobMinionEvents_JobMinionId",
                table: "JobMinionEvents");

            migrationBuilder.DropColumn(
                name: "JobMinionId",
                table: "JobMinionEvents");

            migrationBuilder.DropColumn(
                name: "ReturnCode",
                table: "JobMinionEvents");

            migrationBuilder.DropColumn(
                name: "ReturnData",
                table: "JobMinionEvents");

            migrationBuilder.DropColumn(
                name: "Success",
                table: "JobMinionEvents");

            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "JobMinionEvents");

            migrationBuilder.AddColumn<string>(
                name: "Data",
                table: "JobMinionEvents",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "JobMinionEvents",
                nullable: false,
                defaultValue: "");
        }
    }
}
