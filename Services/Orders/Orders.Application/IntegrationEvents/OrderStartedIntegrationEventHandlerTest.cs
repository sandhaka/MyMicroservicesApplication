using System;
using System.Threading.Tasks;
using EventBus.Abstractions;
using Orders.Application.IntegrationEvents.Events;

namespace Orders.Application.IntegrationEvents
{
    public class OrderStartedIntegrationEventHandlerTest : IIntegrationEventHandler<OrderStartedIntegrationEvent>
    {
        public Task Handle(OrderStartedIntegrationEvent @event)
        {
            var task = new Task(() =>
            {
                Console.WriteLine($"Event received! Order Id: {@event.OrderId}, Date: {@event.CreationDate}");
            });
            task.Start();
            return task;
        }
    }
}