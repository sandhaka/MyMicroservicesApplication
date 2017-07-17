using System;
using Amazon.SQS;
using EventBus.Abstractions;

namespace EventBusAwsSqs
{
    public class EventBusAwsSqs : IEventBus
    {
        private readonly IAmazonSQS _amazonSqs;

        public EventBusAwsSqs(IAmazonSQS amazonSqs)
        {
            _amazonSqs = amazonSqs ?? throw new ArgumentNullException(nameof(amazonSqs));
        }
    }
}