using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IntegrationEventsContext.Models;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace IntegrationEventsContext
{
    public class IntegrationEventsRespository : IIntegrationEventsRespository
    {
        private readonly ConnectionMultiplexer _connectionMultiplexer;

        public IntegrationEventsRespository(ConnectionMultiplexer connectionMultiplexer)
        {
            _connectionMultiplexer = connectionMultiplexer;
        }
        
        public async Task<IntegrationEvent> GetModelAsync<T>()
        {
            string eventType = typeof(T).Name;
            var database = _connectionMultiplexer.GetDatabase();
            
            var data = await database.StringGetAsync(eventType);
            if (data.IsNullOrEmpty)
            {
                return null;
            }
            
            return JsonConvert.DeserializeObject<IntegrationEvent>(data);
        }

        public async Task<IntegrationEvent> SetModelAsync<T, TH>(bool subscription = true)
        {
            var eventType = typeof(T).Name;
            var subscriberName = typeof(TH).Name;
 
            var database = _connectionMultiplexer.GetDatabase();

            var model = await GetModelAsync<T>();
            if (model == null)
            {
                model = new IntegrationEvent(eventType);
            }
                       
            if (model.Subscribers.All(i => i.Item1 != subscriberName))
            {
                if (subscription)
                {
                    model.Subscribers.Add(new Tuple<string, bool>(subscriberName, false));
                }
            }
            else
            {
                if (!subscription)
                {
                    model.Subscribers.Remove(model.Subscribers.First(i => i.Item1 == subscriberName));
                }
            }
            
            var created = await database.StringSetAsync(eventType, JsonConvert.SerializeObject(model));     
            if (!created)
            {
                return null;
            }

            return await GetModelAsync<T>();
        }

        public async Task<IntegrationEventInstance> CreateInstanceAsync<T>()
        {
            string eventType = typeof(T).Name;
            var database = _connectionMultiplexer.GetDatabase();

            var guid = Guid.NewGuid().ToString();
            
            // Debug
            Console.WriteLine("Created new integration event instance with guid: " + guid);

            var model = await GetModelAsync<T>();
            if (model == null)
            {
                return null;
            }

            var instance = new IntegrationEventInstance(guid, eventType)
            {
                Subscribers = model.Subscribers
            };

            var created = await database.StringSetAsync(guid, JsonConvert.SerializeObject(instance));
            if (!created)
            {
                return null;
            }
            
            var eInstances = await GetIntegrationEventRegisteredInstances();
            if (eInstances == null)
            {
                eInstances = new Dictionary<string, string>();
            }
            
            eInstances.Add(instance.Id, instance.EventType);
            
            eInstances = await SetIntegrationEventRegisteredInstances(eInstances);
            
            if (eInstances == null)
            {
                //TODO: log and thorw excpetion
            }

            return await GetInstanceAsync<T>(guid);
        }

        public async Task<bool> DeleteInstanceAsync(string guid, string eventType)
        {
            var database = _connectionMultiplexer.GetDatabase();
            
            var eInstances = await GetIntegrationEventRegisteredInstances();

            eInstances.Remove(guid);
            
            eInstances = await SetIntegrationEventRegisteredInstances(eInstances);
            
            if (eInstances == null)
            {
                //TODO: log and thorw excpetion
            }

            return await database.KeyDeleteAsync(guid);
        }
        
        public async Task<IntegrationEventInstance> GetInstanceAsync(string guid, string eventType)
        {
            var database = _connectionMultiplexer.GetDatabase();
            
            var data = await database.StringGetAsync(guid);
            if (data.IsNullOrEmpty)
            {
                return null;
            }
            
            return JsonConvert.DeserializeObject<IntegrationEventInstance>(data);
        }

        public async Task<IntegrationEventInstance> MarkSubscriberHandlerAsProcessed(
            string guid, 
            string eventTypeName, 
            string subscriberName)
        {
            var inst = await GetInstanceAsync(guid, eventTypeName);
            var sub = inst.Subscribers.First(i => i.Item1 == subscriberName);
            var newValue = new Tuple<string, bool>(sub.Item1, true);
            inst.Subscribers.Remove(sub);
            inst.Subscribers.Add(newValue);
            return await UpdateInstanceAsync(inst);
        }

        public async Task<IDictionary<string, string>> GetIntegrationEventRegisteredInstances()
        {
            var database = _connectionMultiplexer.GetDatabase();

            var instances = await database.StringGetAsync("iEvents.Instances");
            if (instances.IsNullOrEmpty)
            {
                return null;
            }
            
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(instances);
        }
        
        private async Task<IntegrationEventInstance> GetInstanceAsync<T>(string guid)
        {
            string eventType = typeof(T).Name;
            return await GetInstanceAsync(guid, eventType);
        }

        private async Task<IDictionary<string, string>> SetIntegrationEventRegisteredInstances(
            IDictionary<string, string> instances)
        {
            var database = _connectionMultiplexer.GetDatabase();

            var data = JsonConvert.SerializeObject(instances);

            var created = await database.StringSetAsync("iEvents.Instances", data);
            if (!created)
            {
                return null;
            }

            return await GetIntegrationEventRegisteredInstances();
        }
        
        private async Task<IntegrationEventInstance> UpdateInstanceAsync(IntegrationEventInstance instance)
        {
            var database = _connectionMultiplexer.GetDatabase();

            var created = await database.StringSetAsync(instance.Id, JsonConvert.SerializeObject(instance));
            if (!created)
            {
                return null;
            }

            return await GetInstanceAsync(instance.Id, instance.EventType);
        }
    }
}