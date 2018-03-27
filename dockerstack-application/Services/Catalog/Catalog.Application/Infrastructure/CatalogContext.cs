using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Catalog.Application.DbModels;

namespace Catalog.Application.Infrastructure
{
    public class CatalogContext : DbContext
    {
        private const string DefaultSchema = "mymicsapp.Services.catalogDb";

        public CatalogContext(DbContextOptions<CatalogContext> options) : base(options) {}
        
        public DbSet<Product> Products { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(ConfigureProducts);
        }
        
        private void ConfigureProducts(EntityTypeBuilder<Product> productConfiguration)
        {
            productConfiguration.ToTable("products", DefaultSchema);
            productConfiguration.HasKey(cr => cr.Id);
            productConfiguration.Property<string>("ProductName").IsRequired();
            productConfiguration.Property<decimal>("UnitPrice").IsRequired();
            productConfiguration.Property<string>("Package").IsRequired();
            productConfiguration.Property<int>("Assets").IsRequired();
        }
    }
}