using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.DbModels
{
    /// <summary>
    /// Database context
    /// </summary>
    public class IdentityContext : DbContext
    {
        private const string DefaultSchema = "mymicsapp.Services.identityDb";
        
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
            appUserConfiguration.Property<string>("username").IsRequired();
            appUserConfiguration.Property<string>("password").IsRequired();         
            appUserConfiguration.Property<string>("name_full");         
            appUserConfiguration.Property<int>("level");         
        }
    }
}
