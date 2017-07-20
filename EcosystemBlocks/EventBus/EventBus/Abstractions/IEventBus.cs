using System;
using System.Threading;
using System.Threading.Tasks;
using EventBus.Events;
using EventBus.Responses;

namespace EventBus.Abstractions
{
    public interface IEventBus
    {
        Task<EbPublishResponse> PublishAsync<T>(string message, CancellationToken cancellationToken)
            where T : IntegrationEvent;

        string Subscribe<T, TH>(Func<TH> handler)
            where T : IntegrationEvent 
            where TH : IIntegrationEventHandler<T>;

        bool Unsubscribe<T>(string guid)
            where T : IntegrationEvent;
    }
}