using System.Threading.Tasks;
using MediatR;
using Orders.Domain.AggregatesModel.Events;

namespace Orders.Application.DomainEventsHandler
{
    /// <summary>
    /// Update order with the verified payment information
    /// </summary>
    public class UpdateOrderOnPaymentMethodVerifiedEventHandler : 
        IAsyncNotificationHandler<PaymentMethodVerifiedEvent>
    {
        public Task Handle(PaymentMethodVerifiedEvent notification)
        {
            // TODO: update order

            return Task.CompletedTask;
        }
    }
}