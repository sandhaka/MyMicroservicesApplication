using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Orders.Domain.SeedWork;

namespace Orders.Infrastructure
{
    static class MediatorExtension
    {
        /// <summary>
        /// Process all the domain events by the mediator. Important to avoid side-effect on transaction operations.
        /// </summary>
        /// <param name="mediator">Mediator instance</param>
        /// <param name="ctx">Db context</param>
        /// <returns>Task</returns>
        public static async Task DispatchDomainEventsAsync(this IMediator mediator, OrdersContext ctx)
        {
            var domainEntities = ctx.ChangeTracker
                .Entries<Entity>()
                .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any());

            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.DomainEvents)
                .ToList();

            domainEntities.ToList()
                .ForEach(entity => entity.Entity.DomainEvents.Clear());

            var tasks = domainEvents
                .Select(async (domainEvent) => {
                    await mediator.Publish(domainEvent);
                });

            // Ensures the atomicity of the all db changes
            await Task.WhenAll(tasks); 
        }
    }
}