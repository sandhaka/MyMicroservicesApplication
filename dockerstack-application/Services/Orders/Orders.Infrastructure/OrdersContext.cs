using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orders.Domain.AggregatesModel.BuyerAggregate;
using Orders.Domain.AggregatesModel.OrderAggregate;
using Orders.Domain.SeedWork;

namespace Orders.Infrastructure
{
    /// <summary>
    /// Ordering database context
    /// </summary>
    public class OrdersContext : DbContext, IUnitOfWork
    {
        private const string DefaultSchema = "mymicsapp.Services.ordersDb";

        private readonly IMediator _mediator;
        
        public OrdersContext(DbContextOptions<OrdersContext> options, IMediator mediator) : base(options)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Buyer> Buyers { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        
        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            await _mediator.DispatchDomainEventsAsync(this);

            await base.SaveChangesAsync(cancellationToken);

            return true;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>(ConfigureOrder);
            modelBuilder.Entity<OrderItem>(ConfigureOrderItem);
            modelBuilder.Entity<Buyer>(ConfigureBuyer);
            modelBuilder.Entity<PaymentMethod>(ConfigurePayment);
        }

        private void ConfigureOrder(EntityTypeBuilder<Order> orderConfiguration)
        {
            orderConfiguration.ToTable("orders", DefaultSchema);
            orderConfiguration.HasKey(o => o.Id);
            orderConfiguration.Ignore(o => o.DomainEvents);
            orderConfiguration.Property<DateTime>("OrderDate").IsRequired();
            orderConfiguration.Property<int?>("BuyerId").IsRequired(false);
            orderConfiguration.Property<int?>("PaymentMethodId").IsRequired(false);

            var navigation = orderConfiguration.Metadata.FindNavigation(nameof(Order.OrderItems));
            
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);
            
            orderConfiguration.HasOne<PaymentMethod>()
                .WithMany()
                .HasForeignKey("PaymentMethodId")
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            orderConfiguration.HasOne<Buyer>()
                .WithMany()
                .HasForeignKey("BuyerId")
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private void ConfigureOrderItem(EntityTypeBuilder<OrderItem> orderItemConfiguration)
        {
            orderItemConfiguration.ToTable("orderItems", DefaultSchema);
            orderItemConfiguration.HasKey(o => o.Id);
            orderItemConfiguration.Ignore(o => o.DomainEvents);
            orderItemConfiguration.Property<int>("OrderId").IsRequired();            
            orderItemConfiguration.Property<int>("ProductId").IsRequired();
            orderItemConfiguration.Property<string>("ProductName").IsRequired();
            orderItemConfiguration.Property<decimal>("UnitPrice").IsRequired();
            orderItemConfiguration.Property<int>("Units").IsRequired();
        }

        private void ConfigureBuyer(EntityTypeBuilder<Buyer> buyerConfiguration)
        {
            buyerConfiguration.ToTable("buyers", DefaultSchema);
            buyerConfiguration.HasKey(o => o.Id);
            buyerConfiguration.Ignore(o => o.DomainEvents);
            buyerConfiguration.Property<string>(o => o.IdentityGuid)
                .HasMaxLength(200)
                .IsRequired();
            buyerConfiguration.HasIndex("IdentityGuid").IsUnique();
            buyerConfiguration.HasMany(b => b.PaymentMethods)
                .WithOne()
                .HasForeignKey("BuyerId")
                .OnDelete(DeleteBehavior.Cascade);
            
            var navigation = buyerConfiguration.Metadata.FindNavigation(nameof(Buyer.PaymentMethods));
            
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
        
        private void ConfigurePayment(EntityTypeBuilder<PaymentMethod> paymentConfiguration)
        {
            paymentConfiguration.ToTable("paymentmethods", DefaultSchema);
            paymentConfiguration.HasKey(b => b.Id);
            paymentConfiguration.Ignore(b => b.DomainEvents);

            paymentConfiguration.Property<int>("BuyerId").IsRequired();
            paymentConfiguration.Property<string>("CardHolder")
                .HasMaxLength(200)
                .IsRequired();
            paymentConfiguration.Property<string>("CardNumber")
                .HasMaxLength(25)
                .IsRequired();
            paymentConfiguration.Property<DateTime>("Expiration").IsRequired();
        }
    }
}