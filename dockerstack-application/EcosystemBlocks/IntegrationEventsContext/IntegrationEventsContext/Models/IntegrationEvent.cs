using System;
using System.Collections.Generic;
using System.Text;

namespace IntegrationEventsContext.Models
{
    public class IntegrationEvent
    {
        public string EventType { get; }
        
        /// <summary>
        /// Set of subscriber tuples, instance name - handled status
        /// </summary>
        public HashSet<Tuple<string,bool>> Subscribers { get; set; }
        
        public IntegrationEvent(string eventType)
        {
            EventType = eventType;
            Subscribers = new HashSet<Tuple<string, bool>>();
        }

        protected string SubscribersToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            foreach (var subscriber in Subscribers)
            {
                sb.Append($"[Instance name: {subscriber.Item1}, Handled: {subscriber.Item2}]");
            }
            sb.Append("}");
            return sb.ToString();
        }
    }
}