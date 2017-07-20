using System;
using System.Runtime.Serialization;
using EventBus.Events;

namespace Orders.Application.IntegrationEvents.Events
{
    [DataContract]
    public class OrderStartedIntegrationEvent : IntegrationEvent
    {
        [DataMember]
        public int OrderId { get; set; }

        public OrderStartedIntegrationEvent(Guid guid, DateTime creation, int orderId) : base(guid, creation)
        {
            OrderId = orderId;
        }
    }
}