using System;

namespace EventBus
{
    /// <summary>
    /// Aws SNS message DTO
    /// </summary>
    public class SnsMessage
    {
        public string Type { get; set; }
        public string MessageId { get; set; }
        public string TopicArn { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public string Signature { get; set; }
    }
}