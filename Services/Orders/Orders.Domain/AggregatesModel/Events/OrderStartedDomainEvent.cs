using MediatR;
using Orders.Domain.AggregatesModel.OrderAggregate;

namespace Orders.Domain.AggregatesModel.Events
{
    public class OrderStartedDomainEvent: INotification
    {
        public Order Order { get; private set; }

        public OrderStartedDomainEvent(Order order)
        {
            Order = order;
        }
    }
}