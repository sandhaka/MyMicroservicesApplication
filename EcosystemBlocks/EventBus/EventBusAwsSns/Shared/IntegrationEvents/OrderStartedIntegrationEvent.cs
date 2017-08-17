using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using EventBus.Events;

namespace EventBusAwsSns.Shared.IntegrationEvents
{
    [DataContract]
    public class OrderStartedIntegrationEvent : IntegrationEvent
    {
        [DataMember]
        public int OrderId { get; set; }

        [DataMember]
        public List<OrderItemInfo> OrderItems { get; set; }
        
        [DataMember]
        public string UserId { get; }

        public OrderStartedIntegrationEvent(Guid guid, string userId, DateTime creation, int orderId, List<OrderItemInfo> orderItems) 
            : base(guid, creation)
        {
            OrderId = orderId;
            OrderItems = orderItems;
            UserId = userId;
        }
    }
    
    public class OrderItemInfo
    {
        public int ProductId { get; set; }
        public int Assets { get; set; }
    }
}