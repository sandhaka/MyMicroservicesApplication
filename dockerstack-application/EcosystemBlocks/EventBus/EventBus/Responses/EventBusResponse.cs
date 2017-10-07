using System.Net;

namespace EventBus.Responses
{
    public class EventBusResponse
    {
        public EventBusResponse(HttpStatusCode httpStatusCode)
        {
            StatusCode = httpStatusCode;
        }
        
        public HttpStatusCode StatusCode { get; }
        public string Arn { get; set; }
    }
}