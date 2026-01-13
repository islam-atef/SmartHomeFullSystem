using Application.Abstractions.CasheStorage.Mqtt;
using Application.Abstractions.Messaging.Mqtt;
using Application.Contracts.Messaging.Mqtt;
using Infrastructure.Messaging.Mqtt.Parsing;
using Infrastructure.Messaging.Mqtt.Serialization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Messaging.Mqtt
{
    public sealed class DeviceStateListener
    {
        private readonly ILogger<DeviceStateListener> _logger;
        private readonly IUnitStateStore _store;
        private readonly IUnitMessageSerializer _serializer;
        private readonly ITopicParser _parser;
        //private readonly IDevicesRealtimePublisher _realtime;

        public DeviceStateListener(
            IMqttBus bus,
            IUnitStateStore store,
            IUnitMessageSerializer serializer,
            //IDevicesRealtimePublisher realtime,
            ITopicParser parser,
            ILogger<DeviceStateListener> logger)
        {
            _store = store;
            _logger = logger;
            _serializer = serializer;
            _parser = parser;
            //_realtime = realtime;
            
            // invoke in MQTT MessageReceiver event
            bus.MessageReceived += HandleAsync;
        }

        private async Task HandleAsync(string topic, byte[] payload)
        {
            // deal jest with state topics
            if (!topic.EndsWith("/state", StringComparison.OrdinalIgnoreCase))
                return;

            var state = _serializer.DeserializeState(payload);

            if (state is null)
                return;

            // update timestamps
            if (state.LastSeenUtc == default) state.LastSeenUtc = DateTime.UtcNow;
            state.UpdatedAtUtc = DateTime.UtcNow;

            // save in Redis
            await _store.SaveAsync(state.ControlUnitId, state);

            // SignalR Code:-
            /*
             * لازم تكون قادر تستخرج homeId/roomId من الـ topic أو يكونوا داخل الـ payload
             *  مثال سريع: parse from topic (انت الأفضل تعمل Parser)
             */
            var ids = _parser.Parse(topic); // homeId, roomId, unitId
            //await _realtime.PublishUnitStateAsync(ids.HomeId, ids.RoomId, ids.UnitId, state);
        }
    }
}
