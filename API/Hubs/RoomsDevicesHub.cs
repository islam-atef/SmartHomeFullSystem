using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;


namespace API.Hubs
{
    [Authorize]
    public sealed class RoomDevicesHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            // هنا تقدر تعمل logging أو validations
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            // هنا cleanup أو logging
            await base.OnDisconnectedAsync(exception);
        }

        public Task JoinRoom(string homeId, string roomId)
            => Groups.AddToGroupAsync(
                Context.ConnectionId,
                $"home:{homeId}:room:{roomId}");

        public Task LeaveRoom(string homeId, string roomId)
            => Groups.RemoveFromGroupAsync(
                Context.ConnectionId,
                $"home:{homeId}:room:{roomId}");
    }
}
