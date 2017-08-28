using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SimpleNotificationService;
using Amazon.SQS;
using Amazon.SQS.Model;
using EventBus;
using EventBus.Abstractions;
using EventBus.Events;
using EventBus.Responses;
using IntegrationEventsContext;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EventBusAwsSns
{
    public class EventBus : IEventBus
    {
        private readonly ISubscriptionsManager _subscriptionsManager;
        private readonly IAmazonSimpleNotificationService _snsClient;
        private readonly IAmazonSQS _amazonSqsClient;
        private readonly IConfiguration _configuration;
        private readonly IIntegrationEventsRespository _integrationEventsRespository;
        private readonly List<Tuple<string, Thread, CancellationToken>> _pollingThreads;
        private readonly ILogger<EventBus> _logger;
        
        /// <summary>
        /// ctor,
        /// Start a polling thread for each event type configured
        /// At the moment the EventBus dosn't accept the adding of new topics at runtime and it doesn't need that.
        /// </summary>
        public EventBus(
            IConfiguration configuration,
            ISubscriptionsManager subscriptionsManager, 
            IAmazonSimpleNotificationService simpleNotificationServiceClient, 
            IIntegrationEventsRespository integrationEventsRespository,
            IAmazonSQS amazonSqsClient, ILogger<EventBus> logger)
        {
            _subscriptionsManager = subscriptionsManager ?? throw new ArgumentNullException(nameof(subscriptionsManager));
            _snsClient = simpleNotificationServiceClient ?? throw new ArgumentNullException(nameof(simpleNotificationServiceClient));
            _amazonSqsClient = amazonSqsClient ?? throw new ArgumentNullException(nameof(amazonSqsClient));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _integrationEventsRespository = integrationEventsRespository ?? throw new ArgumentNullException(nameof(integrationEventsRespository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            
            _pollingThreads = new List<Tuple<string, Thread, CancellationToken>>();
        }

        /// <summary>
        /// Initialize the event bus
        /// </summary>
        public void Init()
        {
            var topicsConfig = _configuration.GetSection("AwsEventBus:Topics").GetChildren();
            
            // Polling threads
            foreach (var configurationSection in topicsConfig)
            {
                var th = new Thread(Polling);
                var cancellationToken = new CancellationToken();
                
                _pollingThreads.Add(new Tuple<string, Thread, CancellationToken>(
                    configurationSection.Key, th, cancellationToken)
                );
                
                th.Start(new object[] {configurationSection.Key, cancellationToken});
            }
            
            // Handle the delete message request from the bus
            _subscriptionsManager.OnIntegrationEventReadyToDelete += (sender, message) =>
            {
                var response = _amazonSqsClient.DeleteMessageAsync(new DeleteMessageRequest()
                {
                    QueueUrl = message.QueueUrl,
                    ReceiptHandle = message.Message.ReceiptHandle
                }).Result;

                if (response.HttpStatusCode != HttpStatusCode.OK)
                {
                    _logger.LogError($"Error on aws queue message deleting, response metadata: {response.ResponseMetadata}");
                }
            };
        }

        /// <summary>
        /// Publish a new message on the Amazon simple notification service
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <typeparam name="T">Event type</typeparam>
        /// <returns>Publish response</returns>
        public async Task<EbPublishResponse> PublishAsync<T>(string message, CancellationToken cancellationToken)
            where T : IntegrationEvent
        {
            if (!_subscriptionsManager.HasEventTopic<T>())
            {
                return null;
            }

            var topicArn = _subscriptionsManager.GetEventTopicArn<T>();

            var response = await _snsClient.PublishAsync(topicArn, message, cancellationToken);

            if (response.HttpStatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            var instance = await _integrationEventsRespository.CreateInstanceAsync<T>();
            if (instance == null)
            {
                throw new Exception($"Error on creation of the integration event {typeof(T).Name} instance");
            }

            return new EbPublishResponse(response.HttpStatusCode)
            {
                MessageId = response.MessageId
            };
        }

        /// <summary>
        /// Subscribe to the Event type
        /// </summary>
        /// <param name="handler">Handler</param>
        /// <typeparam name="T">Event type</typeparam>
        /// <typeparam name="TH">Handler type</typeparam>
        /// <returns>Subscription guid</returns>
        public string Subscribe<T, TH>(Func<TH> handler) 
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            if (!_subscriptionsManager.HasEventTopic<T>())
            {
                return string.Empty;
            }

            var model = _integrationEventsRespository.SetModelAsync<T, TH>();
            if (model == null)
            {
                throw new Exception($"Error on subscription to the integration event {typeof(T).Name}");
            }
            
            return _subscriptionsManager.AddSubscription<T, TH>(handler);
        }

        /// <summary>
        /// Unsubscribe form the event type T
        /// </summary>
        /// <param name="guid">Subscription guid</param>
        /// <typeparam name="T">Event type</typeparam>
        /// <typeparam name="TH"></typeparam>
        /// <returns>Result</returns>
        public bool Unsubscribe<T, TH>(string guid)
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            if (!_subscriptionsManager.HasEventTopic<T>() || !_subscriptionsManager.HasSubscription<T>(guid))
            {
                return false;
            }

            var model = _integrationEventsRespository.SetModelAsync<T, TH>(false);
            if (model == null)
            {
                throw new Exception($"Error on usubscription to the integration event {typeof(T).Name}");
            }
            
            _subscriptionsManager.RemoveSubscription<T>(guid);

            return !_subscriptionsManager.HasSubscription<T>(guid);
        }

        /// <summary>
        /// Perform polling on the queue
        /// </summary>
        /// <param name="args">Parameters object</param>
        private void Polling(object args)
        {
            var _args = args as object[];
            var topic = _args[0] as string;
            var cancellationToken = (CancellationToken) _args[1];
            
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;
                
                try
                {
                    var queueUrl = _configuration.GetSection("AwsEventBus:Topics:" + topic + ":SqsUrl").Value;
                    var receiveMessageRequest = new ReceiveMessageRequest()
                    {
                        QueueUrl = queueUrl,
                        WaitTimeSeconds = 5,
                        MaxNumberOfMessages = 10
                    };
                    
                    var receivedMessageResponse =
                        _amazonSqsClient.ReceiveMessageAsync(receiveMessageRequest, CancellationToken.None).Result;

                    foreach (var message in receivedMessageResponse.Messages)
                    {
                        _subscriptionsManager.ProcessEvent(topic, 
                            new IntegrationEventReceivedNotificationDto(message, queueUrl));
                    }
                }
                catch(Exception e)
                {
                    _logger.LogError($"Error on polling: {e.Message}, stacktrace: {e.StackTrace}");
                }
            }
        }
    }
}