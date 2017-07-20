using System.Net;

namespace EventBus.Responses
{
    public class EbPublishResponse : EventBusResponse
    {
        public EbPublishResponse(HttpStatusCode httpStatusCode) : base(httpStatusCode) { }
        
        public string MessageId { get; set; }
    }
}