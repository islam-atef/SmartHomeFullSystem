using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contracts.Messaging.Mqtt
{
    public sealed record TopicIds
    (
        Guid HomeId,
        Guid RoomId,
        Guid UnitId
    );
}
