using MediatR;
using Orders.Domain.AggregatesModel.BuyerAggregate;

namespace Orders.Domain.AggregatesModel.Events
{
    public class PaymentMethodVerifiedEvent : INotification
    {
        public Buyer Buyer { get; private set; }
        public PaymentMethod PaymentMethod { get; private set; }
        public int OrderId { get; private set; }
        
        public PaymentMethodVerifiedEvent(Buyer buyer, PaymentMethod payment, int orderId)
        {
            Buyer = buyer;
            PaymentMethod = payment;
            OrderId = orderId;
        }
    }
}