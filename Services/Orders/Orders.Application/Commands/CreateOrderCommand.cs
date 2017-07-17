using System.Collections.Generic;
using System.Runtime.Serialization;
using MediatR;
using Orders.Application.Controllers;

namespace Orders.Application.Commands
{
    /// <summary>
    /// Command create order
    /// </summary>
    [DataContract]
    public class CreateOrderCommand : IRequest<bool>
    {
        [DataMember]
        private readonly List<OrderItemDto> _orderItems;
        
        [DataMember]
        public List<OrderItemDto> OrderItems => _orderItems;

        public CreateOrderCommand()
        {
            _orderItems = new List<OrderItemDto>();
        }
        
        public CreateOrderCommand(List<OrderItemDto> orderItems)
        {
            _orderItems = orderItems;
        }
    }
}