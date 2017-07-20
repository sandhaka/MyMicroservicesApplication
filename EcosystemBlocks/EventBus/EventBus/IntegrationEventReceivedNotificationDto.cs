using Amazon.SQS.Model;

namespace EventBus
{
    public class IntegrationEventReceivedNotificationDto
    {
        public IntegrationEventReceivedNotificationDto(Message message, string queueUrl)
        {
            Message = message;
            QueueUrl = queueUrl;
        }
        
        public Message Message { get; }
        public string QueueUrl { get; }
    }
}