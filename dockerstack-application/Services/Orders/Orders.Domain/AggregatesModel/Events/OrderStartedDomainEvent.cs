using System;
using MediatR;
using Orders.Domain.AggregatesModel.OrderAggregate;

namespace Orders.Domain.AggregatesModel.Events
{
    public class OrderStartedDomainEvent: INotification
    {
        public Order Order { get; private set; }
        public string CardNumber { get; private set; }
        public string CardHolder { get; private set; }
        public string CardSecurityNumber { get; private set; }
        public DateTime CardExpiration { get; private set; }

        public OrderStartedDomainEvent(
            Order order,
            string cardNumber,
            string cardHolder,
            string cardSecurityNumber,
            DateTime cardExpiration)
        {
            Order = order;
            CardExpiration = cardExpiration;
            CardHolder = cardHolder;
            CardNumber = cardNumber;
            CardSecurityNumber = cardSecurityNumber;
        }
    }
}