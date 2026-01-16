using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Realtime
{
    public interface IDevicesRealtimePublisher
    {
        Task UnitStateUpdatedAsync(Guid homeId, Guid roomId, Guid unitId, object state, CancellationToken ct = default);
    }
}
