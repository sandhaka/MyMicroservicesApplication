using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventBus.Abstractions;
using MediatR;
using Newtonsoft.Json;
using Orders.Application.IntegrationEvents.Events;
using Orders.Domain.AggregatesModel.OrderAggregate;

namespace Orders.Application.Commands
{
    /// <summary>
    /// Handler for the create order command
    /// </summary>
    public class CreateOrderCommandHandler 
        : IAsyncRequestHandler<CreateOrderCommand, bool>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IEventBus _eventBus;
        
        public CreateOrderCommandHandler(IOrderRepository orderRepository, IEventBus eventBus)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        }

        /// <summary>
        /// Service handler, create the order and commit it into the persistence layer
        /// </summary>
        /// <param name="createOrderCommand">command</param>
        /// <returns>result</returns>
        public async Task<bool> Handle(CreateOrderCommand createOrderCommand)
        {
            var order = new Order();

            // Add order items
            foreach (var orderItem in createOrderCommand.OrderItems)
            {
                order.AddOrderItem(orderItem.ProductId, orderItem.ProductName, orderItem.UnitPrice, orderItem.Units);
            }
            
            // Save the new order on the db
            _orderRepository.Add(order);
            var result = await _orderRepository.UnitOfWork.SaveEntitiesAsync();

            // Publish new order as integration event
            var items = new List<OrderItemInfo>();
            order.OrderItems.ToList().ForEach((item) =>
            {
                items.Add(new OrderItemInfo() {ProductId = item.ProductId, Assets = item.Units});
            });
            var integrationEvent = new OrderStartedIntegrationEvent(Guid.NewGuid(), DateTime.UtcNow, order.Id, items);
            
            await _eventBus.PublishAsync<OrderStartedIntegrationEvent>(
                JsonConvert.SerializeObject(integrationEvent), 
                CancellationToken.None
            );

            return result;
        }
    }
}