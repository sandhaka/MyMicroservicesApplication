using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventBusAwsSns.Shared.IntegrationEvents;
using MediatR;
using Microsoft.Extensions.Logging;
using Orders.Application.Services;
using Orders.Domain.AggregatesModel.Events;
using Orders.Domain.AggregatesModel.OrderAggregate;

namespace Orders.Application.DomainEventsHandler
{
    /// <summary>
    /// Update order with the verified payment information
    /// </summary>
    public class UpdateOrderOnPaymentMethodVerifiedEventHandler : 
        IAsyncNotificationHandler<PaymentMethodVerifiedEvent>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILoggerFactory _logger;
        private readonly IOrderingIntegrationEventService _orderingIntegrationEventService;
        
        public UpdateOrderOnPaymentMethodVerifiedEventHandler(
            IOrderRepository orderRepository, 
            IOrderingIntegrationEventService orderingIntegrationEventService,
            ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory;
            _orderRepository = orderRepository;
            _orderingIntegrationEventService = orderingIntegrationEventService;
        }
        
        public async Task Handle(PaymentMethodVerifiedEvent paymentMethodVerifiedEvent)
        {
            var order = await _orderRepository.GetAsync(paymentMethodVerifiedEvent.OrderId);
            order.SetBuyerId(paymentMethodVerifiedEvent.Buyer.Id);
            order.SetPaymentMethodId(paymentMethodVerifiedEvent.PaymentMethod.Id);
            
            // Publish the new ordering integration event
            var items = new List<OrderItemInfo>();
            order.OrderItems.ToList().ForEach((item) =>
            {
                items.Add(new OrderItemInfo() {ProductId = item.ProductId, Assets = item.Units});
            });
            
            var integrationEvent = new OrderStartedIntegrationEvent(
                Guid.NewGuid(), paymentMethodVerifiedEvent.Buyer.IdentityGuid, DateTime.UtcNow, items);

            await _orderingIntegrationEventService.PublishThroughEventBusAsync(integrationEvent);

            await _orderingIntegrationEventService.SaveEventAndOrderingContextChangesAsync();
            
            _logger.CreateLogger(nameof(UpdateOrderOnPaymentMethodVerifiedEventHandler))
                .LogTrace($"Order with Id: {paymentMethodVerifiedEvent.OrderId} has been successfully updated with a payment method id: { paymentMethodVerifiedEvent.PaymentMethod.Id }");
        }
    }
}