using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace HomeWallet_API.Migrations
{
    public partial class Newuserfields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Language",
                schema: "dbo",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Theme",
                schema: "dbo",
                table: "Users",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Language",
                schema: "dbo",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Theme",
                schema: "dbo",
                table: "Users");
        }
    }
}
