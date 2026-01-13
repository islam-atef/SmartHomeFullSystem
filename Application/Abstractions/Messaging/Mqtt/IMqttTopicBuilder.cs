using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Messaging.Mqtt
{
    public interface IMqttTopicBuilder
    {
        // publish
        string ControlUnitCommand(Guid homeId, Guid roomId, Guid controlUnitId);

        // subscribe filters
        string ControlUnitState(); // global
        string ControlUnitState(Guid homeId, Guid roomId, Guid controlUnitId);

        string ControlUnitTelemetry(); // global
        string ControlUnitTelemetry(Guid homeId, Guid roomId, Guid controlUnitId);

        string RoomWildcard(Guid homeId, Guid roomId);
    }
}
