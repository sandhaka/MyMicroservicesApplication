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
        public Dictionary<int, int> OrderItems { get; set; }

        public OrderStartedIntegrationEvent(Guid guid, DateTime creation, int orderId, Dictionary<int,int> orderItems) 
            : base(guid, creation)
        {
            OrderId = orderId;
            OrderItems = orderItems;
        }
    }
}