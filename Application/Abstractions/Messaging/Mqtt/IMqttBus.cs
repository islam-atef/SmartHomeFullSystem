using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Messaging.Mqtt
{
    public interface IMqttBus
    {
        Task PublishAsync(
            string topic,
            string payload,
            bool retain = false,
            int qos = 1,
            CancellationToken ct = default);

        Task PublishAsync(
            string topic,
            byte[] payload,
            bool retain = false,
            int qos = 1,
            CancellationToken ct = default);

        Task SubscribeAsync(
            string topicFilter,
            int qos = 1,
            CancellationToken ct = default);

        Task UnsubscribeAsync(
            string topicFilter,
            CancellationToken ct = default);

        /// <summary>
        /// Fired when a message is received from the broker.
        /// topic: full topic name
        /// payload: raw bytes
        /// </summary>
        event Func<string, byte[], Task>? MessageReceived;
    }
}
