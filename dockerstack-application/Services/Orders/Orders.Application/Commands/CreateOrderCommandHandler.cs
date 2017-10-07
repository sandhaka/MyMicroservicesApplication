using System;
using System.Threading.Tasks;
using EventBus.Abstractions;
using MediatR;
using Orders.Domain.AggregatesModel.OrderAggregate;

namespace Orders.Application.Commands
{
    /// <summary>
    /// Create a new order
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
        /// Create the order and commit it into the persistence layer
        /// </summary>
        /// <param name="command">command</param>
        /// <returns>result</returns>
        public async Task<bool> Handle(CreateOrderCommand command)
        {
            var order = new Order(
                command.CardNumber, 
                command.CardSecurityNumber, 
                command.CardHolderName, 
                command.CardExpiration, 
                command.BuyerId, 
                command.PaymentId);

            // Add order items
            foreach (var orderItem in command.OrderItems)
            {
                order.AddOrderItem(orderItem.ProductId, orderItem.ProductName, orderItem.UnitPrice, orderItem.Units);
            }
            
            // Save the new order on the db
            _orderRepository.Add(order);
            
            var result = await _orderRepository.UnitOfWork.SaveEntitiesAsync();

            return result;
        }
    }
}