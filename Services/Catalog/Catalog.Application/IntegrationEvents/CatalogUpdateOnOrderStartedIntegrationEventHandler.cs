using System.Threading.Tasks;
using Catalog.Application.Infrastructure.Repositories;
using EventBus.Abstractions;
using EventBusAwsSns.Shared.IntegrationEvents;
using Microsoft.Extensions.Logging;

namespace Catalog.Application.IntegrationEvents
{
    public class CatalogUpdateOnOrderStartedIntegrationEventHandler : IIntegrationEventHandler<OrderStartedIntegrationEvent>
    {
        private readonly ILogger _logger;
        private readonly ICatalogRepository _catalogRepository;

        public CatalogUpdateOnOrderStartedIntegrationEventHandler(
            ILogger<CatalogUpdateOnOrderStartedIntegrationEventHandler> iLogger, 
            ICatalogRepository catalogRepository)
        {
            _catalogRepository = catalogRepository;
            _logger = iLogger;
        }
        
        public Task Handle(OrderStartedIntegrationEvent @event)
        {
            _logger.LogInformation($"Received integration event Order started, " +
                                   $"Creation date: {@event.CreationDate}, " +
                                   $"Update catalog");
            
            return _catalogRepository.UpdateProductsAssetsAsync(@event.OrderItems);
        }
    }
}