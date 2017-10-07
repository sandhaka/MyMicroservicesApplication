using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Orders.Application.Migrations
{
    public partial class AddingBuyerAndPaymentIDtoOrderEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BuyerId",
                schema: "mymicsapp.Services.ordersDb",
                table: "orders",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaymentMethodId",
                schema: "mymicsapp.Services.ordersDb",
                table: "orders",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BuyerId",
                schema: "mymicsapp.Services.ordersDb",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "PaymentMethodId",
                schema: "mymicsapp.Services.ordersDb",
                table: "orders");
        }
    }
}
