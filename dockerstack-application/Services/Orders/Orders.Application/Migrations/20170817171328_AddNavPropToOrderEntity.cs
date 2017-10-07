using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Orders.Application.Migrations
{
    public partial class AddNavPropToOrderEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_orders_BuyerId",
                schema: "mymicsapp.Services.ordersDb",
                table: "orders",
                column: "BuyerId");

            migrationBuilder.CreateIndex(
                name: "IX_orders_PaymentMethodId",
                schema: "mymicsapp.Services.ordersDb",
                table: "orders",
                column: "PaymentMethodId");

            migrationBuilder.AddForeignKey(
                name: "FK_orders_buyers_BuyerId",
                schema: "mymicsapp.Services.ordersDb",
                table: "orders",
                column: "BuyerId",
                principalSchema: "mymicsapp.Services.ordersDb",
                principalTable: "buyers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_orders_paymentmethods_PaymentMethodId",
                schema: "mymicsapp.Services.ordersDb",
                table: "orders",
                column: "PaymentMethodId",
                principalSchema: "mymicsapp.Services.ordersDb",
                principalTable: "paymentmethods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_orders_buyers_BuyerId",
                schema: "mymicsapp.Services.ordersDb",
                table: "orders");

            migrationBuilder.DropForeignKey(
                name: "FK_orders_paymentmethods_PaymentMethodId",
                schema: "mymicsapp.Services.ordersDb",
                table: "orders");

            migrationBuilder.DropIndex(
                name: "IX_orders_BuyerId",
                schema: "mymicsapp.Services.ordersDb",
                table: "orders");

            migrationBuilder.DropIndex(
                name: "IX_orders_PaymentMethodId",
                schema: "mymicsapp.Services.ordersDb",
                table: "orders");
        }
    }
}
