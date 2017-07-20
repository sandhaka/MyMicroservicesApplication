using System;
using System.Runtime.Serialization;

namespace EventBus.Events
{
    /// <summary> 
    /// Integration Events notes: 
    // An Event is “something that has happened in the past”, therefore its name has to be   
    // An Integration Event is an event that can cause side effects to other microsrvices, Bounded-Contexts or external systems.
    /// </summary>
    [DataContract]
    public class IntegrationEvent
    {
        [DataMember]
        public Guid Id { get; set; }
        [DataMember]
        public DateTime CreationDate { get; set; }
        
        public IntegrationEvent(Guid eventGuid, DateTime creationDateTime)
        {
            Id = eventGuid;
            CreationDate = creationDateTime;
        }
    }
}