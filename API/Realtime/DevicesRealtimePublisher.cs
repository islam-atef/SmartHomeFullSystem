using API.Hubs;
using Application.Abstractions.Realtime;
using Microsoft.AspNetCore.SignalR;

namespace API.Realtime
{
    public sealed class DevicesRealtimePublisher : IDevicesRealtimePublisher
    {
        private readonly IHubContext<RoomDevicesHub> _hub;

        public DevicesRealtimePublisher(IHubContext<RoomDevicesHub> hub)
        {
            _hub = hub;
        }

        public Task UnitStateUpdatedAsync(
            Guid homeId,
            Guid roomId,
            Guid unitId,
            object state,
            CancellationToken ct = default)
        {
            var group = $"home:{homeId}:room:{roomId}";

            return _hub.Clients.Group(group)
                .SendAsync("UnitStateUpdated", new
                {
                    homeId,
                    roomId,
                    unitId,
                    state
                }, ct);
        }
    }
}
