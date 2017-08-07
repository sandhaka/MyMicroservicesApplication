using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using EventBus.Events;

namespace Catalog.Application.IntegrationEvents.Events
{
    [DataContract]
    public class OrderStartedIntegrationEvent : IntegrationEvent
    {
        [DataMember]
        public int OrderId { get; set; }

        [DataMember]
        public List<OrderItemInfo> OrderItems { get; set; }

        public OrderStartedIntegrationEvent(Guid guid, DateTime creation, int orderId, List<OrderItemInfo> orderItems) 
            : base(guid, creation)
        {
            OrderId = orderId;
            OrderItems = orderItems;
        }
    }
    
    public class OrderItemInfo
    {
        public int ProductId { get; set; }
        public int Assets { get; set; }
    }
}