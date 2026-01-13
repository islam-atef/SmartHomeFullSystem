using Application.Contracts.Messaging.Mqtt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Messaging.Mqtt.Serialization
{
    public class UnitMessageSerializer : IUnitMessageSerializer
    {
        private static readonly JsonSerializerOptions Options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public ControlUnitState? DeserializeState(byte[] payload)
        {
            try
            {
                return JsonSerializer.Deserialize<ControlUnitState>(payload, Options);
            }
            catch
            {
                return null;
            }
        }

        public byte[] SerializeCommand(ControlUnitCommand command)
        {
            return JsonSerializer.SerializeToUtf8Bytes(command, Options);
        }
    }
}
