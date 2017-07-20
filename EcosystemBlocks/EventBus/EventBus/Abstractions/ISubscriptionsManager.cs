using System;
using EventBus.Events;

namespace EventBus.Abstractions
{
    public interface ISubscriptionsManager
    {
        event EventHandler<IntegrationEventReceivedNotificationDto> OnIntegrationEventReceived;
            
        string AddSubscription<T, TH>(Func<TH> handler)
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>;

        void RemoveSubscription<T>(string guid)
            where T : IntegrationEvent;
        
        bool HasSubscription<T>(string guid)
            where T : IntegrationEvent;
        
        string GetEventTopicArn<T>() where T : IntegrationEvent;

        bool HasEventTopic<T>() where T : IntegrationEvent;

        string GetEventKey<T>() where T : IntegrationEvent;

        void ProcessEvent(string typeName, IntegrationEventReceivedNotificationDto eventReceived);
    }
}