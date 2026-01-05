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

        public string ControlUnitCommand(Guid homeId, Guid roomId, Guid controlUnitId)
        {
            return $"{Root}/{homeId}/rooms/{roomId}/units/{controlUnitId}/cmd";
        }

        public string ControlUnitState(Guid homeId, Guid roomId, Guid controlUnitId)
        {
            return $"{Root}/{homeId}/rooms/{roomId}/units/{controlUnitId}/state";
        }

        public string ControlUnitTelemetry(Guid homeId, Guid roomId, Guid controlUnitId)
        {
            return $"{Root}/{homeId}/rooms/{roomId}/units/{controlUnitId}/telemetry";
        }

        public string RoomWildcard(Guid homeId, Guid roomId)
        {
            // subscribe with all units in the room
            return $"{Root}/{homeId}/rooms/{roomId}/units/+/+";
        }
    }
}
