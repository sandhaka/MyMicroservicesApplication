using System;
using System.Collections.Generic;

namespace IntegrationEventsContext.Models
{
    public class IntegrationEvent
    {
        public string EventType { get; }
        public HashSet<Tuple<string,bool>> Subscribers { get; set; }
        
        public IntegrationEvent(string eventType)
        {
            EventType = eventType;
            Subscribers = new HashSet<Tuple<string, bool>>();
        }
    }
}