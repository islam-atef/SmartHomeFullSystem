using Domain.Entities.SqlEntities.RoomEntities;
using Domain.Entities.SqlEntities.UsersEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.RepositotyInterfaces
{
    public interface IHomeRepo
    {
        Task<Guid> CreateHomeAsync(string name, string ip, double latitude, double longitude, Guid homeOwner);

        Task<Home?> GetHomeAsync(Guid homeId);
        Task<string> GetHomeNameAsync(Guid homeId);
        Task<Guid> GetHomeOwnerAsync(Guid homeId);
        Task<(double Latitude, double Longitude)?> GetHomeLocationAsync(Guid homeId);
        Task<IReadOnlyList<Room>?> GetHomeRoomsAsync(Guid homeId);
        Task<IReadOnlyList<AppUser>?> GetHomeUsersAsync(Guid homeId);

        Task<bool> CheckHomeRomeExistanceAsync(Guid homeId, Guid roomID);
        Task<bool> CheckHomeUserExistanceAsync(Guid homeId, Guid userID);

        Task<bool> RenameHomeAsync(Guid homeId, string newName);
        Task<bool> UpdateHomeLocationAsync(Guid homeId, double latitude, double longitude);
            
        Task<bool> AddHomeUserAsync(Guid homeId, Guid userId);
        Task<bool> RemoveHomeUserAsync(Guid homeId, Guid userId);

        Task<bool> SetHomeIPAsync(Guid homeId, string ip);
        Task<bool> SetHomeOwnerAsync(Guid homeId, Guid userId);

        Task<bool> DeleteHomeAsync(Guid homeId);
    }
}
