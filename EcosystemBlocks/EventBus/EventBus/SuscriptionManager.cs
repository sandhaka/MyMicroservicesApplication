using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EventBus.Abstractions;
using EventBus.Events;
using IntegrationEventsContext;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EventBus
{
    /// <summary>
    /// Subscription manager
    /// </summary>
    public class SuscriptionManager : ISubscriptionsManager
    {
        private readonly Dictionary<string, List<Tuple<string,Delegate>>> _handlers;
        private readonly List<Type> _types;
        private readonly IConfiguration _configuration;
        private readonly IIntegrationEventsRespository _integrationEventsRespository;
        private readonly ILogger _logger;

        public event EventHandler<IntegrationEventReceivedNotificationDto> OnIntegrationEventReceived;
        public event EventHandler<IntegrationEventReceivedNotificationDto> OnIntegrationEventReadyToDelete; 

        public SuscriptionManager(
            IConfiguration configuration, 
            IIntegrationEventsRespository integrationEventsRespository,
            ILogger<SuscriptionManager> logger)
        {
            _handlers = new Dictionary<string, List<Tuple<string,Delegate>>>();
            _types = new List<Type>();
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger;
            _integrationEventsRespository = integrationEventsRespository;
        }
        
        public string AddSubscription<T, TH>(Func<TH> handler) where T : IntegrationEvent where TH : IIntegrationEventHandler<T>
        {
            var key = GetEventKey<T>();
            if (!HasSubscriptionForEvent<T>())
            {
                _handlers.Add(key, new List<Tuple<string,Delegate>>());
            }
            
            string guid = Guid.NewGuid().ToString();
            _handlers[key].Add(new Tuple<string, Delegate>(guid, handler));
            _types.Add(typeof(T));
            return guid;
        }

        public void RemoveSubscription<T>(string guid) where T : IntegrationEvent
        {
            if (!HasSubscription<T>(guid))
            {
                return;
            }
            
            var key = GetEventKey<T>();
            var toRemove = _handlers[key].Find(i => i.Item1 == guid);
            _handlers[key].Remove(toRemove);
        }

        public bool HasSubscription<T>(string guid) where T : IntegrationEvent
        {
            var key = GetEventKey<T>();
            if (_handlers.ContainsKey(key))
            {
                return _handlers[key].FirstOrDefault(i => i.Item1 == guid) != null;
            }
            return false;
        }

        public string GetEventTopicArn<T>() where T : IntegrationEvent
        {
            var type = typeof(T).Name;
            var arn = string.Empty;
            
            switch (type)
            {
                case "OrderStartedIntegrationEvent":
                {
                    arn = _configuration.GetSection("AwsEventBus:Topics:OrderStartedIntegrationEvent:arn").Value;
                    break;
                }
            }

            return arn;
        }

        public string GetEventKey<T>() where T : IntegrationEvent
        {
            return typeof(T).Name;
        }

        // Process the integration events
        public void ProcessEvent(string typeName, IntegrationEventReceivedNotificationDto eventReceived)
        {
            var eventType = GetEventTypeByName(typeName);
            
            var sqsResponse = (SnsMessage)JsonConvert.DeserializeObject(eventReceived.Message.Body, typeof(SnsMessage));

            var integrationEvent = JsonConvert.DeserializeObject(sqsResponse.Message, eventType);

            if (integrationEvent == null)
            {
                return;
                // TODO Add log
            }
            
            OnIntegrationEventReceived?.Invoke(this, eventReceived);
                
            foreach (var handlerCollection in _handlers)
            {
                if (handlerCollection.Key != typeName)
                {
                    continue;
                }
                
                foreach (var handlerValue in handlerCollection.Value)
                {
                    var handlerFactory = handlerValue.Item2;
                    var handler = handlerFactory.DynamicInvoke();
                    var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);

                    var t = concreteType.GetRuntimeMethod("Handle", new[] {eventType});
                    t.Invoke(handler, new [] {integrationEvent});
                    
                    UpdateIntegrationEventContext(eventReceived, eventType, handler.ToString().Split('.').Last());
                }
            }
        }

        public bool HasEventTopic<T>() where T : IntegrationEvent
        {
            return !string.IsNullOrEmpty(GetEventTopicArn<T>());
        }

        private bool HasSubscriptionForEvent<T>() where T : IntegrationEvent
        {
            var key = GetEventKey<T>();
            return _handlers.ContainsKey(key);
        }

        private Type GetEventTypeByName(string typeName) => _types.Single(t => t.Name == typeName);

        private void UpdateIntegrationEventContext(
            IntegrationEventReceivedNotificationDto eventReceived,
            Type eventType, 
            string handlerName)
        {
            var instances = _integrationEventsRespository.GetIntegrationEventRegisteredInstances().Result;

            if (!instances.Values.Contains(eventType.Name))
            {
                OnIntegrationEventReadyToDelete?.Invoke(this, eventReceived);
                return;
            }
            
            var instance = instances.First(i => i.Value == eventType.Name);
                    
            _logger.LogTrace($"Mark sub handler as processed, Guid: {instance.Key}, Event type: {instance.Value}, Handler name: {handlerName}");
            
            var updatedInstance = _integrationEventsRespository.MarkSubscriberHandlerAsProcessed(
                instance.Key,
                instance.Value,
                handlerName).Result;
                    
            if (updatedInstance == null)
            {
                //TODO: throw
            }

            if (updatedInstance.Subscribers.All(i => i.Item2 == true))
            {
                OnIntegrationEventReadyToDelete?.Invoke(this, eventReceived);
                        
                var result = _integrationEventsRespository.DeleteInstanceAsync(updatedInstance.Id,
                    updatedInstance.EventType).Result;
                        
                if (!result)
                {
                    // TODO Throw 
                }
            }
        }
    }
}