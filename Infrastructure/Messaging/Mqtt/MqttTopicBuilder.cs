using Application.Abstractions.Messaging.Mqtt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Messaging.Mqtt
{
    public sealed class MqttTopicBuilder : IMqttTopicBuilder
    {
        private const string Root = "smarthome";

        // publish
        public string ControlUnitCommand(Guid homeId, Guid roomId, Guid controlUnitId)
            => $"{Root}/{homeId}/rooms/{roomId}/units/{controlUnitId}/cmd";


        // subscribe filters
        public string ControlUnitState(Guid homeId, Guid roomId, Guid controlUnitId)
            => $"{Root}/{homeId}/rooms/{roomId}/units/{controlUnitId}/state";

        public string ControlUnitState()
            => $"{Root}/+/rooms/+/units/+/state";


        public string ControlUnitTelemetry(Guid homeId, Guid roomId, Guid controlUnitId)
            => $"{Root}/{homeId}/rooms/{roomId}/units/{controlUnitId}/telemetry";

        public string ControlUnitTelemetry()
            => $"{Root}/+/rooms/+/units/+/telemetry";


        public string RoomWildcard(Guid homeId, Guid roomId)
            => $"{Root}/{homeId}/rooms/{roomId}/units/+/+";// subscribe with all units in the room
    }
}
