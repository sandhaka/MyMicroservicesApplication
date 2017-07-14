using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.DbModels
{
    public class IdentityContext : DbContext
    {
        private const string DefaultSchema = "mymicsapp_identity";
        
        public IdentityContext(DbContextOptions<IdentityContext> options) : base(options) {}
        
        public DbSet<AppUser> ApplicationUsers {get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppUser>(ConfigureAppUser);
        }

        private void ConfigureAppUser(EntityTypeBuilder<AppUser> appUserConfiguration)
        {
            appUserConfiguration.ToTable("applicationusers", DefaultSchema);
            appUserConfiguration.HasKey(cr => cr.id);
            appUserConfiguration.Property(cr => cr.username).IsRequired();
            appUserConfiguration.Property(cr => cr.password).IsRequired();         
        }
    }
}
