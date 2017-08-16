using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Orders.Application.Services;
using Orders.Domain.AggregatesModel.BuyerAggregate;
using Orders.Domain.AggregatesModel.Events;

namespace Orders.Application.DomainEventsHandler
{
    /// <summary>
    /// Add buyer and payment info to the order
    /// </summary>
    public class ValidateOrAddBuyerOnOrderStartedEventHandler : 
        IAsyncNotificationHandler<OrderStartedDomainEvent>
    {
        private readonly ILoggerFactory _logger;
        private readonly IIdentityService _identityService;
        private readonly IBuyerRepository _buyerRepository;

        public ValidateOrAddBuyerOnOrderStartedEventHandler(
            IIdentityService identityService, 
            IBuyerRepository buyerRepository,
            ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory;
            _identityService = identityService;
            _buyerRepository = buyerRepository;
        }
       
        public async Task Handle(OrderStartedDomainEvent notification)
        {
            var guid = _identityService.GetUserIdentity();

            var buyer = await _buyerRepository.FindAsync(guid);
            var buyerOriginallyExisted = (buyer != null);
            
            if (!buyerOriginallyExisted)
            {
                buyer = new Buyer(guid);
            }

            buyer.VerifyOrAddPaymentMethod(
                notification.CardNumber,
                notification.CardSecurityNumber,
                notification.CardHolder,
                notification.CardExpiration,
                notification.Order.Id);

            var buyerUpdated = buyerOriginallyExisted ? _buyerRepository.Update(buyer) : _buyerRepository.Add(buyer);

            await _buyerRepository.UnitOfWork.SaveEntitiesAsync();
            
            _logger.CreateLogger(typeof(ValidateOrAddBuyerOnOrderStartedEventHandler)).LogTrace($"Buyer {buyerUpdated.Id} and related payment method were validated or updated for orderId: {notification.Order.Id}.");
        }
    }
}