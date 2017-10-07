using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Orders.Infrastructure;

namespace Orders.Application.Migrations
{
    [DbContext(typeof(OrdersContext))]
    partial class OrdersContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2");

            modelBuilder.Entity("Orders.Domain.AggregatesModel.BuyerAggregate.Buyer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("IdentityGuid")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.HasIndex("IdentityGuid")
                        .IsUnique();

                    b.ToTable("buyers","mymicsapp.Services.ordersDb");
                });

            modelBuilder.Entity("Orders.Domain.AggregatesModel.BuyerAggregate.PaymentMethod", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("BuyerId");

                    b.Property<string>("CardHolder")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<string>("CardNumber")
                        .IsRequired()
                        .HasMaxLength(25);

                    b.Property<DateTime>("Expiration");

                    b.HasKey("Id");

                    b.HasIndex("BuyerId");

                    b.ToTable("paymentmethods","mymicsapp.Services.ordersDb");
                });

            modelBuilder.Entity("Orders.Domain.AggregatesModel.OrderAggregate.Order", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("BuyerId");

                    b.Property<DateTime>("OrderDate");

                    b.Property<int?>("PaymentMethodId");

                    b.HasKey("Id");

                    b.HasIndex("BuyerId");

                    b.HasIndex("PaymentMethodId");

                    b.ToTable("orders","mymicsapp.Services.ordersDb");
                });

            modelBuilder.Entity("Orders.Domain.AggregatesModel.OrderAggregate.OrderItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("OrderId");

                    b.Property<int>("ProductId");

                    b.Property<string>("ProductName")
                        .IsRequired();

                    b.Property<decimal>("UnitPrice");

                    b.Property<int>("Units");

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.ToTable("orderItems","mymicsapp.Services.ordersDb");
                });

            modelBuilder.Entity("Orders.Domain.AggregatesModel.BuyerAggregate.PaymentMethod", b =>
                {
                    b.HasOne("Orders.Domain.AggregatesModel.BuyerAggregate.Buyer")
                        .WithMany("PaymentMethods")
                        .HasForeignKey("BuyerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Orders.Domain.AggregatesModel.OrderAggregate.Order", b =>
                {
                    b.HasOne("Orders.Domain.AggregatesModel.BuyerAggregate.Buyer")
                        .WithMany()
                        .HasForeignKey("BuyerId");

                    b.HasOne("Orders.Domain.AggregatesModel.BuyerAggregate.PaymentMethod")
                        .WithMany()
                        .HasForeignKey("PaymentMethodId");
                });

            modelBuilder.Entity("Orders.Domain.AggregatesModel.OrderAggregate.OrderItem", b =>
                {
                    b.HasOne("Orders.Domain.AggregatesModel.OrderAggregate.Order")
                        .WithMany("OrderItems")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
