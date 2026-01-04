using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contracts.Messaging.Mqtt
{
    public sealed class ControlUnitState 
    {
        public ControlUnitState() { }
       
        // identity
        public Guid ControlUnitId { get; init; }

        // power
        public bool IsOn { get; set; }

        // connection
        public bool IsOnline { get; set; }
        public DateTime LastSeenUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }

        // safety
        public string? ErrorCode { get; set; }

        // optional common controls
        public int? Brightness { get; set; }      // 0–100
        public int? Temperature { get; set; }     // AC / Heater
        public int? Speed { get; set; }            // Fan speed

        // unit info
        public string? FirmwareVersion { get; set; }
        public string? Status { get; set; }
    }
}
