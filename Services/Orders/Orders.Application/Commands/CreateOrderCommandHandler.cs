using System;
using System.Threading.Tasks;
using MediatR;
using Orders.Domain.AggregatesModel.OrderAggregate;

namespace Orders.Application.Commands
{
    /// <summary>
    /// Handler for the create order command
    /// </summary>
    public class CreateOrderCommandHandler 
        : IAsyncRequestHandler<CreateOrderCommand, bool>
    {
        private readonly IMediator _mediator;
        private readonly IOrderRepository _orderRepository;
        
        public CreateOrderCommandHandler(IMediator mediator, IOrderRepository orderRepository)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        }

        /// <summary>
        /// Service handler, create the order and commit it into the persistence layer
        /// </summary>
        /// <param name="createOrderCommand">command</param>
        /// <returns>result</returns>
        public async Task<bool> Handle(CreateOrderCommand createOrderCommand)
        {
            var order = new Order();

            foreach (var orderItem in createOrderCommand.OrderItems)
            {
                order.AddOrderItem(orderItem.ProductId, orderItem.ProductName, orderItem.UnitPrice, orderItem.Units);
            }

            _orderRepository.Add(order);

            return await _orderRepository.UnitOfWork.SaveEntitiesAsync();
        }
    }
}