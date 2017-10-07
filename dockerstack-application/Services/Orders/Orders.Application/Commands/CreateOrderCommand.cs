using System;
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
        
        [DataMember]
        public int PaymentId { get; private set; }

        [DataMember]
        public int BuyerId { get; private set; }
        
        [DataMember]
        public string CardNumber { get; private set; }

        [DataMember]
        public string CardHolderName { get; private set; }

        [DataMember]
        public DateTime CardExpiration { get; private set; }

        [DataMember]
        public string CardSecurityNumber { get; private set; }

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