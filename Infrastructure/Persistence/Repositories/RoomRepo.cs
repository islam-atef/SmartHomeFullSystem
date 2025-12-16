using Application.Entities.SqlEntities.RoomEntities;
using Domain.RepositotyInterfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class RoomRepo(AppDbContext context, ILogger<RoomRepo> logger) : IRoomRepo
    {
        public async Task<bool> AddHomeRoomAsync(Guid homeId, string roomName)
        {
            if (string.IsNullOrWhiteSpace(roomName) || homeId == Guid.Empty)
            {
                logger.LogWarning("RoomRepo: AddHomeRoomAsync: Missing Data! roomName: {x}, homeId: {y}", roomName, homeId);
                return false; 
            }
            try
            {
                var home = await context.Homes
                    .Include(h => h.HomeRooms)
                    .FirstOrDefaultAsync(h => h.Id == homeId);

                if (home == null)
                {
                    logger.LogError("RoomRepo: AddHomeRoomAsync: No Home with this Id, homeId: {y}", homeId);
                    return false;
                }

                var room = Room.Create(roomName, homeId);
                home.AddRoom(room);
                var result = await context.SaveChangesAsync();
                if(result > 0) 
                    return true;

                logger.LogError("RoomRepo: AddHomeRoomAsync: Database Error While saving!");
                return false;
            }
            catch (Exception ex)
            {
                logger.LogCritical("RoomRepo: AddHomeRoomAsync: {y}", ex.Message);
                return false;
            }
        }

        public async Task<bool> RemoveHomeRoomAsync(Guid roomId)
        {
            if (roomId == Guid.Empty)
            {
                logger.LogWarning("RoomRepo: RemoveHomeRoomAsync: Missing Data! roomId: {y}", roomId);
                return false;
            }
            try
            {
                var room = await context.Rooms.FindAsync(roomId);
                if (room == null)
                {
                    logger.LogError("RoomRepo: RemoveHomeRoomAsync: No Room with this Id, roomId: {y}", roomId);
                    return false;
                }
                context.Rooms.Remove(room);
                var result = await context.SaveChangesAsync();
                if (result > 0)
                    return true;

                logger.LogError("RoomRepo: RemoveHomeRoomAsync: Database Error While Deleting!");
                return false;
            }
            catch (Exception ex)
            {
                logger.LogCritical("RoomRepo: RemoveHomeRoomAsync: {y}", ex.Message);
                return false;
            }
        }
    }
}
