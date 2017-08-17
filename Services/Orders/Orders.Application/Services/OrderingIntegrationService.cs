using System;
using System.Threading;
using System.Threading.Tasks;
using EventBus;
using EventBus.Abstractions;
using EventBus.Events;
using EventBusAwsSns.Shared.IntegrationEvents;
using Newtonsoft.Json;
using Orders.Infrastructure;

namespace Orders.Application.Services
{
    public class OrderingIntegrationService : IOrderingIntegrationEventService
    {
        private readonly OrdersContext _ordersContext;
        private readonly IEventBus _eventBus;

        public OrderingIntegrationService(OrdersContext ordersContext,  IEventBus eventBus)
        {
            _ordersContext = ordersContext;
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        }
        
        public async Task SaveEventAndOrderingContextChangesAsync()
        {
            //Use of an EF Core resiliency strategy when using multiple DbContexts within an explicit BeginTransaction():
            //See: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency            
            await ResilientTransaction.New(_ordersContext)
                .ExecuteAsync(async () => {
                    // Achieving atomicity between original ordering database operation and the IntegrationEventLog thanks to a local transaction
                    await _ordersContext.SaveChangesAsync();
                });
        }

        public async Task PublishThroughEventBusAsync(IntegrationEvent integrationEvent)
        {
            await _eventBus.PublishAsync<OrderStartedIntegrationEvent>(
                JsonConvert.SerializeObject(integrationEvent), 
                CancellationToken.None
            );
        }
    }
}