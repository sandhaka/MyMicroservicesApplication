using System.Threading.Tasks;
using EventBus.Events;

namespace Orders.Application.Services
{
    public interface IOrderingIntegrationEventService
    {
        Task SaveEventAndOrderingContextChangesAsync();
        Task PublishThroughEventBusAsync(IntegrationEvent evt);
    }
}