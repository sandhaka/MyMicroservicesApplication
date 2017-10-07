using System.Collections.Generic;
using System.Threading.Tasks;
using IntegrationEventsContext.Models;

namespace IntegrationEventsContext
{
    public interface IIntegrationEventsRespository
    {
        Task<IntegrationEvent> GetModelAsync<T>();
        Task<IntegrationEvent> SetModelAsync<T, TH>(bool subscription = true);
        
        Task<IntegrationEventInstance> CreateInstanceAsync<T>();
        Task<IntegrationEventInstance> GetInstanceAsync(string guid, string eventType);
        Task<bool> DeleteInstanceAsync(string guid, string eventType);

        Task<IntegrationEventInstance> MarkSubscriberHandlerAsProcessed(
            string guid, 
            string eventTypeName, 
            string subscriberName);

        Task<IDictionary<string, string>> GetIntegrationEventRegisteredInstances();

    }
}