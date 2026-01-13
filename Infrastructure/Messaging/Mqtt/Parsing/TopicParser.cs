using Application.Contracts.Messaging.Mqtt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Messaging.Mqtt.Parsing
{
    public class TopicParser : ITopicParser
    {
        public TopicIds Parse(string topic)
        {
            // مثال:
            // smarthome/1/rooms/2/units/3/state
            var parts = topic.Split('/', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 6)
                throw new FormatException($"Invalid MQTT topic format: {topic}");

            // indexes حسب الـ scheme
            // 0: smarthome
            // 1: homeId
            // 2: rooms
            // 3: roomId
            // 4: units
            // 5: unitId
            // 6: state / telemetry / cmd

            if (!Guid.TryParse(parts[1], out var homeId))
                throw new FormatException("Invalid homeId in topic");

            if (!Guid.TryParse(parts[3], out var roomId))
                throw new FormatException("Invalid roomId in topic");

            if (!Guid.TryParse(parts[5], out var unitId))
                throw new FormatException("Invalid unitId in topic");

            return new TopicIds(homeId, roomId, unitId);
        }
    }
}
