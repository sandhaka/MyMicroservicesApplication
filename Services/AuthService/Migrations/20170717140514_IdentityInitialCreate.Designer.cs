using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using AuthService.DbModels;

namespace AuthService.Migrations
{
    [DbContext(typeof(IdentityContext))]
    [Migration("20170717140514_IdentityInitialCreate")]
    partial class IdentityInitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2");

            modelBuilder.Entity("AuthService.DbModels.AppUser", b =>
                {
                    b.Property<string>("id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("level");

                    b.Property<string>("name_full");

                    b.Property<string>("password")
                        .IsRequired();

                    b.Property<string>("username")
                        .IsRequired();

                    b.HasKey("id");

                    b.ToTable("applicationusers","mymicsapp.Services.identityDb");
                });
        }
    }
}
