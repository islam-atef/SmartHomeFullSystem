using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Messaging.Mqtt
{
    public interface IMqttTopicBuilder
    {
        string ControlUnitCommand(Guid homeId, Guid roomId, Guid controlUnitId);
        string ControlUnitState(Guid homeId, Guid roomId, Guid controlUnitId);
        string ControlUnitTelemetry(Guid homeId, Guid roomId, Guid controlUnitId);

        string RoomWildcard(Guid homeId, Guid roomId);
    }
}
