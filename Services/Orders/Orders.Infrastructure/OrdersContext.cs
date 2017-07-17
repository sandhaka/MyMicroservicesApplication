using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
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
        
        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            await _mediator.DispatchDomainEventsAsync(this);

            var result = await base.SaveChangesAsync(cancellationToken);

            return true;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>(ConfigureOrder);
            modelBuilder.Entity<OrderItem>(ConfigureOrderItem);
        }

        private void ConfigureOrder(EntityTypeBuilder<Order> orderConfiguration)
        {
            orderConfiguration.ToTable("orders", DefaultSchema);
            orderConfiguration.HasKey(o => o.Id);
            orderConfiguration.Ignore(o => o.DomainEvents);
            orderConfiguration.Property<DateTime>("OrderDate").IsRequired();

            var navigation = orderConfiguration.Metadata.FindNavigation(nameof(Order.OrderItems));
            
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);
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
    }
}