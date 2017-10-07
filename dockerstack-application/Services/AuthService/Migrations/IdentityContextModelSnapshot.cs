using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using AuthService.DbModels;

namespace AuthService.Migrations
{
    [DbContext(typeof(IdentityContext))]
    partial class IdentityContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
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
