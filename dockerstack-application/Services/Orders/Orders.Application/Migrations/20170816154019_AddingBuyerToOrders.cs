using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Orders.Application.Migrations
{
    public partial class AddingBuyerToOrders : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "buyers",
                schema: "mymicsapp.Services.ordersDb",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAdd", true),
                    IdentityGuid = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_buyers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "paymentmethods",
                schema: "mymicsapp.Services.ordersDb",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAdd", true),
                    BuyerId = table.Column<int>(nullable: false),
                    CardHolder = table.Column<string>(maxLength: 200, nullable: false),
                    CardNumber = table.Column<string>(maxLength: 25, nullable: false),
                    Expiration = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_paymentmethods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_paymentmethods_buyers_BuyerId",
                        column: x => x.BuyerId,
                        principalSchema: "mymicsapp.Services.ordersDb",
                        principalTable: "buyers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_buyers_IdentityGuid",
                schema: "mymicsapp.Services.ordersDb",
                table: "buyers",
                column: "IdentityGuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_paymentmethods_BuyerId",
                schema: "mymicsapp.Services.ordersDb",
                table: "paymentmethods",
                column: "BuyerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "paymentmethods",
                schema: "mymicsapp.Services.ordersDb");

            migrationBuilder.DropTable(
                name: "buyers",
                schema: "mymicsapp.Services.ordersDb");
        }
    }
}
