using Application.Abstractions.Messaging.Mqtt;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Messaging.Mqtt
{
    public class MqttBusService : IMqttBus
    {
        private readonly IManagedMqttClient _client;
        private readonly ILogger<MqttBusService> _logger;

        public event Func<string, byte[], Task>? MessageReceived;

        public MqttBusService(IManagedMqttClient client, ILogger<MqttBusService> logger)
        {
            _client = client;
            _logger = logger;

            _client.ApplicationMessageReceivedAsync += async e =>
            {
                var topic = e.ApplicationMessage.Topic;
                var payload = e.ApplicationMessage.PayloadSegment.ToArray();

                try
                {
                    var handler = MessageReceived;
                    if (handler is not null)
                        await handler(topic, payload);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "MqttBusService: MessageReceived handler threw an exception. Topic={Topic}", topic);
                }
            };
        }

        public Task PublishAsync(
            string topic,
            string payload,
            bool retain = false,
            int qos = 1,
            CancellationToken ct = default)
        {
            var bytes = Encoding.UTF8.GetBytes(payload);
            return PublishAsync(topic, bytes, retain, qos, ct);
        }

        public Task PublishAsync(
            string topic,
            byte[] payload,
            bool retain = false,
            int qos = 1,
            CancellationToken ct = default)
        {
            var msg = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload)
                .WithRetainFlag(retain)
                .WithQualityOfServiceLevel(ToQos(qos))
                .Build();

            // Managed client: enqueue (works even during reconnect; it queues)
            return _client.EnqueueAsync(msg);
        }

        public Task SubscribeAsync(
            string topicFilter,
            int qos = 1,
            CancellationToken ct = default)
        {
            var filter = new MqttTopicFilterBuilder()
                .WithTopic(topicFilter)
                .WithQualityOfServiceLevel(ToQos(qos))
                .Build();

            _logger.LogInformation("MqttBusService: SubscribeAsync: Subscribing to {TopicFilter}", topicFilter);

            // MQTTnet v5 expects IEnumerable<MqttTopicFilter>
            return _client.SubscribeAsync(new[] { filter });
        }

        public Task UnsubscribeAsync(
            string topicFilter,
            CancellationToken ct = default)
        {
            _logger.LogInformation("MqttBusService: UnsubscribeAsync: Unsubscribing from {TopicFilter}", topicFilter);
            return _client.UnsubscribeAsync(topicFilter);
        }

        private static MqttQualityOfServiceLevel ToQos(int qos)
          => qos switch
          {
              0 => MqttQualityOfServiceLevel.AtMostOnce,
              1 => MqttQualityOfServiceLevel.AtLeastOnce,
              2 => MqttQualityOfServiceLevel.ExactlyOnce,
              _ => MqttQualityOfServiceLevel.AtLeastOnce
          };
    }
}
