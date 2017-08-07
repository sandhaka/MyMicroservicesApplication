using System.Linq;
using System.Threading.Tasks;
using Catalog.Application.Infrastructure.Repositories;
using Catalog.Application.IntegrationEvents.Events;
using EventBus.Abstractions;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Microsoft.Extensions.Logging;

namespace Catalog.Application.IntegrationEvents
{
    public class OrderStartedIntegrationEventHandler : IIntegrationEventHandler<OrderStartedIntegrationEvent>
    {
        private readonly ILogger _logger;
        private readonly ICatalogRepository _catalogRepository;

        public OrderStartedIntegrationEventHandler(ILogger<OrderStartedIntegrationEventHandler> iLogger, ICatalogRepository catalogRepository)
        {
            _catalogRepository = catalogRepository;
            _logger = iLogger;
        }
        
        public Task Handle(OrderStartedIntegrationEvent @event)
        {
            _logger.LogInformation($"Received integration event Order started. " +
                                   $"OrderId: {@event.OrderId}, " +
                                   $"Creation date: {@event.CreationDate}");
            
            return _catalogRepository.UpdateProductsAssetsAsync(@event.OrderItems);
        }
    }
}