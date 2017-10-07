using System.Threading.Tasks;
using Basket.Application.Infrastructure.Repositories;
using Basket.Application.Services;
using EventBus.Abstractions;
using EventBusAwsSns.Shared.IntegrationEvents;
using Microsoft.Extensions.Logging;

namespace Basket.Application.IntegrationEvents
{
    public class DeleteBasketOnOrderStartedIntegrationEventHandler : IIntegrationEventHandler<OrderStartedIntegrationEvent>
    {
        private readonly ILogger _logger;
        private readonly IBasketRepository _basketRepository;
        private readonly IIdentityService _identityService;

        public DeleteBasketOnOrderStartedIntegrationEventHandler(
            ILogger<DeleteBasketOnOrderStartedIntegrationEventHandler> logger, 
            IBasketRepository basketRepository,
            IIdentityService identityService)
        {
            _basketRepository = basketRepository;
            _logger = logger;
            _identityService = identityService;
        }
        
        public Task Handle(OrderStartedIntegrationEvent @event)
        {
            _logger.LogInformation($"Received integration event Order started, " +
                                   $"Creation date: {@event.CreationDate}, " +
                                   "Delete basket");

            return _basketRepository.DeleteBasketAsync(@event.UserId);
        }
    }
}