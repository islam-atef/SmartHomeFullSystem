using Application.Contracts.Messaging.Mqtt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Messaging.Mqtt.Serialization
{
    public interface IUnitMessageSerializer
    {
        ControlUnitState? DeserializeState(byte[] payload); 
        byte[] SerializeCommand(ControlUnitCommand command); 
    }
}
