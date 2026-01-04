using Application.Abstractions.Identity;
using Application.Abstractions.Image;
using Application.Abstractions.Time;
using Application.Contracts.HomeService.DTOs;
using Application.Entities.SqlEntities.RoomEntities;
using Application.GenericResult;
using Application.Home_Management.DTOs;
using Application.Home_Management.Interfaces;
using Application.RepositotyInterfaces;
using Application.User_Dashboard.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Home_Management.Services
{
    public class HomeService : IHomeService
    {
        private readonly IUnitOfWork _work;
        private readonly ILogger<HomeService> _logger;

        public HomeService(
            IUnitOfWork work,
            ILogger<HomeService> logger)
        {
            _work = work;
            _logger = logger;
        }

        public async Task<GenericResult<bool>> CreateNewHomeAsync(CreateHomeDTO homeDTO)
        {
            if (homeDTO == null)
            {
                _logger.LogWarning("HomeService: CreateNewHomeAsync: No data provided!");
                return GenericResult<bool>.Failure(ErrorType.MissingData, "");
            }
            try
            {
                // 1- create home entity
                var result = await _work.Home.CreateHomeAsync(
                    homeDTO.Name,
                    homeDTO.HomeIP ?? "0.0.0.0",
                    homeDTO.Latitude,
                    homeDTO.Longitude,
                    homeDTO.OwnerId);

                if(result)
                    return GenericResult<bool>.Success(true);

                _logger.LogError("HomeService: CreateNewHomeAsync: Error in the HomeRepo");
                return GenericResult<bool>.Failure(ErrorType.DatabaseError, "");
            }
            catch (Exception ex)
            {
                _logger.LogCritical("HomeService: CreateNewHomeAsync: {x}!", ex.Message);
                return GenericResult<bool>.Failure(ErrorType.Conflict, ex.Message);
            }
        }

        public async Task<GenericResult<bool>> UpdateHomeNameAsync(RenameHomeDTO homeDTO)
        {
            if (homeDTO == null)
            {
                _logger.LogWarning("HomeService: UpdateHomeNameAsync: No data provided!");
                return GenericResult<bool>.Failure(ErrorType.MissingData, "");
            }
            try
            {
                var homeOwnerId = await _work.Home.GetHomeOwnerAsync(homeDTO.HomeId);
                if(homeOwnerId != homeDTO.OwnerId)
                {
                    _logger.LogWarning("HomeService: UpdateHomeNameAsync: Unauthorized user try to change home information, UserId {x}", homeDTO.OwnerId);
                    return GenericResult<bool>.Failure(ErrorType.Unauthorized, "");
                }

                var result = await _work.Home.RenameHomeAsync(homeDTO.HomeId, homeDTO.NewName);
                if (result)
                    return GenericResult<bool>.Success(true);

                _logger.LogError("HomeService: UpdateHomeNameAsync: Error in the HomeRepo");
                return GenericResult<bool>.Failure(ErrorType.DatabaseError, "");
            }
            catch (Exception ex)
            {
                _logger.LogCritical("HomeService: UpdateHomeNameAsync: {x}!", ex.Message);
                return GenericResult<bool>.Failure(ErrorType.Conflict, ex.Message);
            }
        }

        public async Task<GenericResult<HomeDataDTO>> GetHomeDataAsync(Guid homeId)
        {
            if(homeId == Guid.Empty)
            {
                _logger.LogWarning("HomeService: GetHomeDataAsync: Missing Data, HomeId: {x} ", homeId);
                return GenericResult<HomeDataDTO>.Failure(ErrorType.MissingData);
            }
            try
            {
                var home = await _work.Home.GetHomeAsync(homeId);
                if(home == null)
                {
                    _logger.LogWarning("HomeService: GetHomeDataAsync: No home found, HomeId: {x} ", homeId);
                    return GenericResult<HomeDataDTO>.Failure(ErrorType.NotFound);
                }

                List<HomeRoomsDTO> homeRooms =new List<HomeRoomsDTO>();
                foreach (var room in home.HomeRooms)
                {
                    var Room = new HomeRoomsDTO { RoomId = room.Id, RoomName = room.RoomName };
                    homeRooms.Add(Room);
                }

                var ownerName = await _work.AppUser.GetUserIdentityInfoAsync(home.HomeOwnerId);

                List<string> userNames = new List<string>();
                foreach(var user in home.AppUsers)
                {
                    var User = await _work.AppUser.GetUserIdentityInfoAsync(user.Id);
                    userNames.Add(User.Value.userName);
                }


                var homeData = new HomeDataDTO 
                {
                    HomeId = homeId,
                    HomeName = home.Name,
                    OwnerName = ownerName.Value.userName,
                    Latitude = home.Latitude,
                    Longitude = home.Longitude,
                    HomeRooms = homeRooms,
                    HomeUsers = userNames,
                };

                return GenericResult<HomeDataDTO>.Success(homeData);
            }
            catch (Exception ex)
            {
                _logger.LogCritical("HomeService: GetHomeDataAsync: {x} ", ex.Message);
                return GenericResult<HomeDataDTO>.Failure(ErrorType.Conflict);
            }
        }

        public async Task<GenericResult<bool>> AddRoomToHomeAsync(CreateHomeRoomDTO roomDTO)
        {
            if (roomDTO == null)
            {
                _logger.LogWarning("HomeService: AddRoomToHomeAsync: No data provided!");
                return GenericResult<bool>.Failure(ErrorType.MissingData, "");
            }
            try
            {
                var homeOwnerId = await _work.Home.GetHomeOwnerAsync(roomDTO.HomeId);
                if (homeOwnerId != roomDTO.OwnerId)
                {
                    _logger.LogWarning("HomeService: AddRoomToHomeAsync: Unauthorized user try to change home information, UserId {x}", roomDTO.OwnerId);
                    return GenericResult<bool>.Failure(ErrorType.Unauthorized, "");
                }

                var result = await _work.Room.AddHomeRoomAsync(roomDTO.HomeId, roomDTO.RoomName);
                if (result)
                    return GenericResult<bool>.Success(true);

                _logger.LogError("HomeService: AddRoomToHomeAsync: Error in the HomeRepo");
                return GenericResult<bool>.Failure(ErrorType.DatabaseError, "");
            }
            catch (Exception ex)
            {
                _logger.LogCritical("HomeService: AddRoomToHomeAsync: {x}!", ex.Message);
                return GenericResult<bool>.Failure(ErrorType.Conflict, ex.Message);
            }
        }
        public async Task<GenericResult<bool>> AddUserToHomeAsync(HomeUser userDTO)
        {
            if (userDTO == null)
            {
                _logger.LogWarning("HomeService: AddUserToHomeAsync: No data provided!");
                return GenericResult<bool>.Failure(ErrorType.MissingData, "");
            }
            try
            {
                var homeOwnerId = await _work.Home.GetHomeOwnerAsync(userDTO.HomeId);
                if (homeOwnerId != userDTO.OwnerId)
                {
                    _logger.LogWarning("HomeService: AddUserToHomeAsync: Unauthorized user try to change home information, UserId {x}", userDTO.OwnerId);
                    return GenericResult<bool>.Failure(ErrorType.Unauthorized, "");
                }

                var result = await _work.Home.AddHomeUserAsync(userDTO.HomeId, userDTO.NewUserId);
                if (result)
                {
                    await _work.HomeSubscription.AcceptRequestAsync(userDTO.HomeId, userDTO.NewUserId);
                    return GenericResult<bool>.Success(true); 
                }

                _logger.LogError("HomeService: AddUserToHomeAsync: Error in the HomeRepo");
                return GenericResult<bool>.Failure(ErrorType.DatabaseError, "");
            }
            catch (Exception ex)
            {
                _logger.LogCritical("HomeService: AddUserToHomeAsync: {x}!", ex.Message);
                return GenericResult<bool>.Failure(ErrorType.Conflict, ex.Message);
            }
        }

        public async Task<GenericResult<bool>> DeleteHomeRoomAsync(DeleteHomeRoomDTO roomDTO)
        {
            if (roomDTO == null)
            {
                _logger.LogWarning("HomeService: DeleteHomeRoomAsync: No data provided!");
                return GenericResult<bool>.Failure(ErrorType.MissingData, "");
            }
            try
            {
                var homeOwnerId = await _work.Home.GetHomeOwnerAsync(roomDTO.HomeId);
                if (homeOwnerId != roomDTO.OwnerId)
                {
                    _logger.LogWarning("HomeService: DeleteHomeRoomAsync: Unauthorized user try to change home information, UserId {x}", roomDTO.OwnerId);
                    return GenericResult<bool>.Failure(ErrorType.Unauthorized, "");
                }
                if ( await _work.Home.CheckHomeRomeExistanceAsync(roomDTO.HomeId, roomDTO.RoomId))
                {
                    _logger.LogWarning("HomeService: DeleteHomeRoomAsync: there is no Room with this Id in this Home, room {x}, home {y}", roomDTO.RoomId, roomDTO.HomeId);
                    return GenericResult<bool>.Failure(ErrorType.Unauthorized, "");
                }
                var result = await _work.Room.RemoveHomeRoomAsync( roomDTO.RoomId);
                if (result)
                    return GenericResult<bool>.Success(true);

                _logger.LogError("HomeService: DeleteHomeRoomAsync: Error in the HomeRepo");
                return GenericResult<bool>.Failure(ErrorType.DatabaseError, "");
            }
            catch (Exception ex)
            {
                _logger.LogCritical("HomeService: DeleteHomeRoomAsync: {x}!", ex.Message);
                return GenericResult<bool>.Failure(ErrorType.Conflict, ex.Message);
            }
        }
        public async Task<GenericResult<bool>> DeleteUserFromHomeAsync(HomeUser userDTO) 
        {
            if (userDTO == null)
            {
                _logger.LogWarning("HomeService: DeleteUserToHomeAsync: No data provided!");
                return GenericResult<bool>.Failure(ErrorType.MissingData, "");
            }
            try
            {
                var homeOwnerId = await _work.Home.GetHomeOwnerAsync(userDTO.HomeId);
                if (homeOwnerId != userDTO.OwnerId)
                {
                    _logger.LogWarning("HomeService: DeleteUserToHomeAsync: Unauthorized user try to change home information, UserId {x}", userDTO.OwnerId);
                    return GenericResult<bool>.Failure(ErrorType.Unauthorized, "");
                }

                var result = await _work.Home.RemoveHomeUserAsync(userDTO.HomeId, userDTO.NewUserId);
                if (result)
                    return GenericResult<bool>.Success(true);

                _logger.LogError("HomeService: DeleteUserToHomeAsync: Error in the HomeRepo");
                return GenericResult<bool>.Failure(ErrorType.DatabaseError, "");
            }
            catch (Exception ex)
            {
                _logger.LogCritical("HomeService: DeleteUserToHomeAsync: {x}!", ex.Message);
                return GenericResult<bool>.Failure(ErrorType.Conflict, ex.Message);
            }
        }

        public async Task<GenericResult<IReadOnlyList<HomeSubscriptionRequestDTO>>> GerHomeAllSubscriptionRequestAsync(Guid homeId, Guid ownerId)
        {
            if (homeId == Guid.Empty || ownerId == Guid.Empty)
            {
                _logger.LogWarning("HomeService: GerHomeAllSubscriptionRequestAsync: Missing input data, homeId: {x}, ownerId: {y}.", homeId, ownerId);
                return GenericResult<IReadOnlyList<HomeSubscriptionRequestDTO>>.Failure(ErrorType.MissingData, "");
            }
            try
            {
                var homeOwnerId = await _work.Home.GetHomeOwnerAsync(homeId);
                if (homeOwnerId != ownerId)
                {
                    _logger.LogWarning("HomeService: GerHomeAllSubscriptionRequestAsync: Unauthorized user try to change home information, UserId {x}", ownerId);
                    return GenericResult<IReadOnlyList<HomeSubscriptionRequestDTO>>.Failure(ErrorType.Unauthorized, "");
                }

                var requests = await _work.HomeSubscription.GetAllHomeRequesstAsync(homeId);
                if (requests != null)
                {
                    var homeName = await _work.Home.GetHomeNameAsync(homeId);
                    List<HomeSubscriptionRequestDTO> result = new List<HomeSubscriptionRequestDTO>();
                    foreach (var request in requests)
                    {
                        HomeSubscriptionRequestDTO rq = new HomeSubscriptionRequestDTO();
                        rq.requestId = request.Id;
                        rq.HomeId = homeId;
                        rq.HomeName = homeName;
                        rq.UserId = request.UserId;
                        var UserInfo = await _work.AppUser.GetUserIdentityInfoAsync(request.UserId);
                        rq.UserName = UserInfo.Value.userName;
                        rq.UserEmail = UserInfo.Value.email;
                        rq.RequestDate = request.CreatedAt;

                        result.Add(rq);
                    }
                    return GenericResult<IReadOnlyList<HomeSubscriptionRequestDTO>>.Success(result);
                }
                return GenericResult<IReadOnlyList<HomeSubscriptionRequestDTO>>.Failure(ErrorType.NotFound);
            }
            catch (Exception ex)
            {
                _logger.LogCritical("HomeService: GerHomeAllSubscriptionRequestAsync: {x}!", ex.Message);
                return GenericResult<IReadOnlyList<HomeSubscriptionRequestDTO>>.Failure(ErrorType.Conflict, ex.Message);
            }
        }
        public async Task<GenericResult<IReadOnlyList<HomeSubscriptionRequestDTO>>> GerHomeNewSubscriptionRequestAsync(Guid homeId, Guid ownerId)
        {
            if (homeId == Guid.Empty || ownerId == Guid.Empty)
            {
                _logger.LogWarning("HomeService: GerHomeNewSubscriptionRequestAsync: Missing input data, homeId: {x}, ownerId: {y}.", homeId, ownerId);
                return GenericResult<IReadOnlyList<HomeSubscriptionRequestDTO>>.Failure(ErrorType.MissingData, "");
            }
            try
            {
                var homeOwnerId = await _work.Home.GetHomeOwnerAsync(homeId);
                if (homeOwnerId != ownerId)
                {
                    _logger.LogWarning("HomeService: GerHomeNewSubscriptionRequestAsync: Unauthorized user try to change home information, UserId {x}", ownerId);
                    return GenericResult<IReadOnlyList<HomeSubscriptionRequestDTO>>.Failure(ErrorType.Unauthorized, "");
                }

                var requests = await _work.HomeSubscription.GetAllHomeRequesstAsync(homeId,(x => x.RequestState == false));
                if (requests != null)
                {
                    var homeName = await _work.Home.GetHomeNameAsync(homeId);
                    List<HomeSubscriptionRequestDTO> result = new List<HomeSubscriptionRequestDTO>();
                    foreach (var request in requests)
                    {
                        HomeSubscriptionRequestDTO rq = new HomeSubscriptionRequestDTO();
                        rq.requestId = request.Id;
                        rq.HomeId = homeId;
                        rq.HomeName = homeName;
                        rq.UserId = request.UserId;
                        var UserInfo = await _work.AppUser.GetUserIdentityInfoAsync(request.UserId);
                        rq.UserName = UserInfo.Value.userName;
                        rq.UserEmail = UserInfo.Value.email;
                        rq.RequestDate = request.CreatedAt;

                        result.Add(rq);
                    }
                    return GenericResult<IReadOnlyList<HomeSubscriptionRequestDTO>>.Success(result);
                }
                return GenericResult<IReadOnlyList<HomeSubscriptionRequestDTO>>.Failure(ErrorType.NotFound);
            }
            catch (Exception ex)
            {
                _logger.LogCritical("HomeService: GerHomeNewSubscriptionRequestAsync: {x}!", ex.Message);
                return GenericResult<IReadOnlyList<HomeSubscriptionRequestDTO>>.Failure(ErrorType.Conflict, ex.Message);
            }
        }


    }
}
