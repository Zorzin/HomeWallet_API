using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace HomeWallet_API.Migrations
{
    public partial class Updateuser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Mode",
                schema: "dbo",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Role",
                schema: "dbo",
                table: "Users");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Mode",
                schema: "dbo",
                table: "Users",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Role",
                schema: "dbo",
                table: "Users",
                nullable: true);
        }
    }
}
