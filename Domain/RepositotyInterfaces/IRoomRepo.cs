using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.RepositotyInterfaces
{
    public interface IRoomRepo
    {
        Task<bool> AddHomeRoomAsync(Guid homeId, string roomName);
        Task<bool> RemoveHomeRoomAsync(Guid roomId);
    }
}
