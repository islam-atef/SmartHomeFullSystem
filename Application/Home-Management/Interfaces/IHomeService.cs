using Application.Contracts.HomeService.DTOs;
using Application.Entities.SqlEntities.RoomEntities;
using Application.GenericResult;
using Application.Home_Management.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Home_Management.Interfaces
{
    public interface IHomeService
    {
        Task<GenericResult<bool>> CreateNewHomeAsync(CreateHomeDTO homeDTO);

        Task<GenericResult<bool>> UpdateHomeNameAsync(RenameHomeDTO homeDTO);

        Task<GenericResult<bool>> AddRoomToHomeAsync(CreateHomeRoomDTO roomDTO);
        Task<GenericResult<bool>> AddUserToHomeAsync(HomeUser userDTO);

        Task<GenericResult<bool>> DeleteHomeRoomAsync(DeleteHomeRoomDTO roomDTO);
        Task<GenericResult<bool>> DeleteUserFromHomeAsync(HomeUser userDTO);

        Task<GenericResult<HomeDataDTO>> GetHomeDataAsync(Guid homeId);
        Task<GenericResult<IReadOnlyList<HomeSubscriptionRequestDTO>>> GerHomeAllSubscriptionRequestAsync(Guid homeId, Guid ownerId);
        Task<GenericResult<IReadOnlyList<HomeSubscriptionRequestDTO>>> GerHomeNewSubscriptionRequestAsync(Guid homeId, Guid ownerId);
    }
}
