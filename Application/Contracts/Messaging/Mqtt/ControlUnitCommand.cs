using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contracts.Messaging.Mqtt
{
    public sealed class ControlUnitCommand
    {
        // identity
        public Guid ControlUnitId { get; init; }

        // power
        public bool? SetPower { get; set; }

        // optional controls
        public int? SetBrightness { get; set; }    // 0–100
        public int? SetTemperature { get; set; }   // AC / Heater
        public int? SetSpeed { get; set; }          // Fan

        // metadata
        public DateTime IssuedAtUtc { get; init; } = DateTime.UtcNow;

        // optional safety / ops
        public string? CorrelationId { get; init; }
    }
}
