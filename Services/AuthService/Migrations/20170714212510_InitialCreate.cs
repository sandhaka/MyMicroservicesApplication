using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AuthService.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "mymicsapp_identity");

            migrationBuilder.CreateTable(
                name: "applicationusers",
                schema: "mymicsapp_identity",
                columns: table => new
                {
                    id = table.Column<string>(nullable: false),
                    level = table.Column<int>(nullable: false),
                    name_full = table.Column<string>(nullable: true),
                    password = table.Column<string>(nullable: false),
                    username = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_applicationusers", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "applicationusers",
                schema: "mymicsapp_identity");
        }
    }
}
