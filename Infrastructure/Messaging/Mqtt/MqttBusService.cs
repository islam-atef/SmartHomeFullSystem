using Application.Abstractions.Messaging.Mqtt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Messaging.Mqtt
{
    public class MqttBusService : IMqttBus
    {
        public event Func<string, byte[], Task>? MessageReceived;

        public Task PublishAsync(string topic, string payload, bool retain = false, int qos = 1, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task PublishAsync(string topic, byte[] payload, bool retain = false, int qos = 1, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task SubscribeAsync(string topicFilter, int qos = 1, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task UnsubscribeAsync(string topicFilter, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}
