using System.Threading.Tasks;
using Catalog.Application.IntegrationEvents.Events;
using EventBus.Abstractions;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Microsoft.Extensions.Logging;

namespace Catalog.Application.IntegrationEvents
{
    public class OrderStartedIntegrationEventHandler : IIntegrationEventHandler<OrderStartedIntegrationEvent>
    {
        private readonly ILogger _logger;

        public OrderStartedIntegrationEventHandler(ILogger<OrderStartedIntegrationEventHandler> iLogger)
        {
            _logger = iLogger;
        }
        
        public Task Handle(OrderStartedIntegrationEvent @event)
        {
            _logger.LogInformation($"Received integration event Order started. " +
                                   $"OrderId: {@event.OrderId}, " +
                                   $"Creation date: {@event.CreationDate}");
            
            // TODO: Remove items quantity from the catalog

            return Task.CompletedTask;
        }
    }
}