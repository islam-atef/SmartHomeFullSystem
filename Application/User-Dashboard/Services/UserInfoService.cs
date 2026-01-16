using Application.Abstractions.Identity;
using Application.Abstractions.Image;
using Domain.GenericResult;
using Domain.RepositotyInterfaces;
using Application.User_Dashboard.DTOs;
using Application.User_Dashboard.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.User_Dashboard.Services
{
    public class UserInfoService : IUserInfoService
    {
        private readonly IImageService _imageService;
        private readonly IUnitOfWork _work;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserInfoService> _logger;
        private readonly IIdentityManagement _users;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserInfoService(
            IImageService imageService,
            IUnitOfWork work,
            IConfiguration configuration,
            ILogger<UserInfoService> logger,
            IIdentityManagement users,
            IHttpContextAccessor httpContextAccessor)
        {
            _imageService = imageService;
            _work = work;
            _configuration = configuration;
            _logger = logger;
            _users = users;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<GenericResult<UserGeneralInfoDTO>> GetUserInfoAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                _logger.LogWarning("UserInfoService: GetUserInfoAsync: No userId provided");
                return GenericResult<UserGeneralInfoDTO>.Failure(ErrorType.NullableValue, "No User Provided");
            }
            try
            {
                // 1- get the Email and the UserName
                var UserData = await _work.AppUser.GetUserInfoAsync(userId);

                // 2- get the user image Url
                var request = _httpContextAccessor.HttpContext.Request;
                var baseUrl = $"{request.Scheme}://{request.Host}";
                var imgUrl = await _work.AppUserManagement.GetUserImageUrlAsync(userId);
                var fullImageUrl = string.IsNullOrEmpty(imgUrl)
                        ? $"{baseUrl}/General/AnonymousUserImage.png"
                        : $"{baseUrl}{imgUrl}";

                var returnedData = new UserGeneralInfoDTO(
                    UserData.Value.displayName,
                    UserData.Value.email,
                    UserData.Value.userName,
                    UserData.Value.phone,
                    fullImageUrl
                    );

                return GenericResult<UserGeneralInfoDTO>.Success(returnedData);

            }
            catch (Exception ex)
            {
                _logger.LogCritical("UserInfoService: GetUserInfoAsync: {error}", ex.Message);
                return GenericResult<UserGeneralInfoDTO>.Failure(ErrorType.Conflict, ex.Message);
            }
        }

        public async Task<GenericResult<bool>> ChangeUserDisplayNameAsync(string newDisplayName, Guid userId)
        {
            if (userId == Guid.Empty || String.IsNullOrEmpty(newDisplayName))
            {
                _logger.LogWarning("UserInfoService: ChangeUserDisplayNameAsync: there ate missing data : {user}, {name}.", userId,newDisplayName);
                return GenericResult<bool>.Failure(ErrorType.MissingData, "there ate missing data");
            }
            try
            {
                var result = await _work.AppUser.ChangeUserDisplayNameAsync(userId, newDisplayName);
                if (!result.IsSuccess)
                {
                    _logger.LogCritical("UserInfoService: GetUserInfoAsync: {error}", result.ErrorMessage);
                    return GenericResult<bool>.Failure(ErrorType.Conflict, result.ErrorMessage);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogCritical("UserInfoService: GetUserInfoAsync: {error}", ex.Message);
                return GenericResult<bool>.Failure(ErrorType.Conflict, ex.Message);
            }
        }
        public async Task<GenericResult<bool>> ChangeUserNameAsync(string newUserName, Guid userId)
        {
            if (userId == Guid.Empty || String.IsNullOrEmpty(newUserName))
            {
                _logger.LogWarning("UserInfoService: ChangeUserDisplayNameAsync: there ate missing data : {user}, {userName}.", userId, newUserName);
                return GenericResult<bool>.Failure(ErrorType.MissingData, "there ate missing data");
            }
            try
            {   // check if this name is exist in the database
                var res = await _users.CheckUserExistAsync(newUserName, "username");
                if (!res)
                {
                    var result = await _work.AppUser.ChangeUserNameAsync(userId, newUserName);
                    if (!result.IsSuccess)
                    {
                        _logger.LogCritical("UserInfoService: GetUserInfoAsync: {error}", result.ErrorMessage);
                        return GenericResult<bool>.Failure(ErrorType.Conflict, result.ErrorMessage);
                    }
                    return result;
                }
                else
                    return GenericResult<bool>.Failure(ErrorType.Validation, "there is a user with this name"); 
            }
            catch (Exception ex)
            {
                _logger.LogCritical("UserInfoService: GetUserInfoAsync: {error}", ex.Message);
                return GenericResult<bool>.Failure(ErrorType.Conflict, ex.Message);
            }
        }
        public async Task<GenericResult<string?>> ChangeUserImageAsync(IFormFile image, Guid userId)
        {
            if (userId == Guid.Empty)
            {
                _logger.LogWarning("UserInfoService: ChangeUserImageAsync: No userId provided");
                return GenericResult<string?>.Failure(ErrorType.NullableValue,"No User Provided");
            }
            try
            {
                // 1- get the old Image Url
                var oldImageUrl = await _work.AppUserManagement.GetUserImageUrlAsync(userId);

                // 2- upload the image first
                var imageUrl = await _imageService.UploadImagesAsync(image, userId.ToString());
                if(imageUrl == "error" || (image != null && imageUrl == null))
                {
                    _logger.LogError("UserInfoService: ChangeUserImageAsync: the Image service can not save the Image");
                    return GenericResult<string?>.Failure(ErrorType.Conflict, "the Image service can not save the Image at the root files");
                }

                // 3- save the image Url at the AppUser Entity in database.
                var dbUploadResult = await _work.AppUserManagement.AddOrUpdateUserImageUrlAsync(imageUrl, userId);
                if(!dbUploadResult)
                {
                    _logger.LogError("UserInfoService: ChangeUserImageAsync: the User Manager can not save the Image at the Database");
                    return GenericResult<string?>.Failure(ErrorType.DatabaseError, "the User Manager can not save the Image at the Database");
                }

                // 4- delete the old image from the root files
                if(!String.IsNullOrEmpty(oldImageUrl))
                { 
                    _imageService.DeleteImage(oldImageUrl); 
                }

                // finally: return the new Url
                return GenericResult<string?>.Success(imageUrl);
            }
            catch (Exception ex)
            {
                _logger.LogCritical("UserInfoService: ChangeUserImageAsync: {error}",ex.Message);
                return GenericResult<string?>.Failure(ErrorType.Conflict, ex.Message);
            }
        }
        public async Task<GenericResult<bool>> ChangeUserPhoneNumberAsync(string phoneNumber, Guid userId)
        {
            if (userId == Guid.Empty || String.IsNullOrEmpty(phoneNumber))
            {
                _logger.LogWarning("UserInfoService: ChangeUserDisplayNameAsync: there ate missing data : {user}, {phone}.", userId, phoneNumber);
                return GenericResult<bool>.Failure(ErrorType.MissingData, "there ate missing data");
            }
            try
            {
                var result = await _work.AppUser.ChangeUserPhoneNumberAsync(userId, phoneNumber);
                if (!result.IsSuccess)
                {
                    _logger.LogCritical("UserInfoService: GetUserInfoAsync: {error}", result.ErrorMessage);
                    return GenericResult<bool>.Failure(ErrorType.Conflict, result.ErrorMessage);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogCritical("UserInfoService: GetUserInfoAsync: {error}", ex.Message);
                return GenericResult<bool>.Failure(ErrorType.Conflict, ex.Message);
            }
        }

        public async Task<GenericResult<IReadOnlyList<UserHomesDTO>>> GetUserHomesAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                _logger.LogWarning("UserInfoService: GetUserHomesAsync: No userId provided");
                return GenericResult<IReadOnlyList<UserHomesDTO>>.Failure(ErrorType.NullableValue, "No User Provided");
            }
            try
            {
                var homes = await _work.AppUserManagement.GetUserHomesAsync(userId);
                List<UserHomesDTO> homesDTOs = new List<UserHomesDTO>();
                if (homes == null || homes.Count == 0)
                {
                    return GenericResult<IReadOnlyList<UserHomesDTO>>.Success(homesDTOs);
                }
                foreach (var home in homes!)
                {
                    var homeDTO = new UserHomesDTO();
                    homeDTO.HomeId = home.Id;
                    homeDTO.IsOwner = home.HomeOwnerId == userId;

                    homeDTO.HomeName = $"{home!.Name} {home.HomeReference}";
                    homeDTO.HomeInfo = home.HomeInfo;

                    homeDTO.Longitude = home.Longitude;
                    homeDTO.Latitude = home.Latitude;

                    homeDTO.Country = home.Country;
                    homeDTO.State = home.State;
                    homeDTO.Address = home.Address;

                    homesDTOs.Add(homeDTO);
                }
                return GenericResult<IReadOnlyList<UserHomesDTO>>.Success(homesDTOs);
            }
            catch (Exception ex)
            {
                _logger.LogCritical("UserInfoService: GetUserHomesAsync: {error}", ex.Message);
                return GenericResult<IReadOnlyList<UserHomesDTO>>.Failure(ErrorType.Conflict, ex.Message);
            }
        }

        public async Task<GenericResult<bool>> SendHomesubscriptionRequestAsync(Guid userId, Guid homeId) 
        {
            if (userId == Guid.Empty || homeId == Guid.Empty)
            {
                _logger.LogWarning("UserInfoService: SendHomesubscriptionRequestAsync: No data provided");
                return GenericResult<bool>.Failure(ErrorType.NullableValue, "No data Provided");
            }
            try
            {
                var result = await _work.HomeSubscription.CreateRequestAsync(homeId, userId);
                if(result)
                    GenericResult<bool>.Success(result);
                _logger.LogError("UserInfoService: SendHomesubscriptionRequestAsync: Error happened while creating the request at the Repo!");
                return GenericResult<bool>.Failure(ErrorType.DatabaseError);
            }
            catch (Exception ex)
            {
                _logger.LogCritical("UserInfoService: SendHomesubscriptionRequestAsync: {error}", ex.Message);
                return GenericResult<bool>.Failure(ErrorType.Conflict, ex.Message);
            }
        }
        public async Task<GenericResult<bool>> DeleteHomesubscriptionRequestAsync(Guid requestId) 
        {
            if (requestId == Guid.Empty)
            {
                _logger.LogWarning("UserInfoService: DeleteHomesubscriptionRequestAsync: No data provided");
                return GenericResult<bool>.Failure(ErrorType.NullableValue, "No data Provided");
            }
            try
            {
                var result = await _work.HomeSubscription.DeleteRequestAsync(requestId);
                if (result)
                    GenericResult<bool>.Success(result);
                _logger.LogError("UserInfoService: SendHomesubscriptionRequestAsync: Error happened while Deleting the request at the Repo!");
                return GenericResult<bool>.Failure(ErrorType.DatabaseError);
            }
            catch (Exception ex)
            {
                _logger.LogCritical("UserInfoService: DeleteHomesubscriptionRequestAsync: {error}", ex.Message);
                return GenericResult<bool>.Failure(ErrorType.Conflict, ex.Message);
            }
        }

        public async Task<GenericResult<IReadOnlyList<UserSubscriptionRequestDTO>>> GetUserAllHomeSubscriptionRequsetsAsync(Guid userId)
        {  
            if (userId == Guid.Empty)
            {
                _logger.LogWarning("UserInfoService: GetUserAllHomeSubscriptionRequsetsAsync: No data provided");
                return GenericResult<IReadOnlyList<UserSubscriptionRequestDTO>>.Failure(ErrorType.NullableValue, "No data Provided");
            }
            try
            {
                List<UserSubscriptionRequestDTO> requestDTOs = new List<UserSubscriptionRequestDTO>();
                var requests = await _work.HomeSubscription.GetAllUserRequesstAsync(userId);
                foreach (var request in requests!)
                {
                    var requestDTO = new UserSubscriptionRequestDTO();
                    requestDTO.RequestId = request.Id;
                    requestDTO.HomeId = request.HomeId;
                    var homeName = await _work.Home.GetHomeNameAsync(request.HomeId);
                    requestDTO.HomeName = homeName;
                    requestDTO.RequestDate = request.CreatedAt;
                    requestDTO.RequestState = request.RequestState;
                    requestDTOs.Add(requestDTO);
                }
                return GenericResult<IReadOnlyList<UserSubscriptionRequestDTO>>.Success(requestDTOs);
            }
            catch (Exception ex)
            {
                _logger.LogCritical("UserInfoService: GetUserAllHomeSubscriptionRequsetsAsync: {error}", ex.Message);
                return GenericResult<IReadOnlyList<UserSubscriptionRequestDTO>>.Failure(ErrorType.Conflict, ex.Message);
            }
        }
        public async Task<GenericResult<IReadOnlyList<UserSubscriptionRequestDTO>>> GetUserNewHomeSubscriptionRequsetsAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                _logger.LogWarning("UserInfoService: GetUserNewHomeSubscriptionRequsetsAsync: No data provided");
                return GenericResult<IReadOnlyList<UserSubscriptionRequestDTO>>.Failure(ErrorType.NullableValue, "No data Provided");
            }
            try
            {
                List<UserSubscriptionRequestDTO> requestDTOs = new List<UserSubscriptionRequestDTO>();
                var requests = await _work.HomeSubscription.GetAllUserRequesstAsync(userId,(r => r.RequestState == false));
                foreach (var request in requests!)
                {
                    var requestDTO = new UserSubscriptionRequestDTO();
                    requestDTO.RequestId = request.Id;
                    requestDTO.HomeId = request.HomeId;
                    var homeName = await _work.Home.GetHomeNameAsync(request.HomeId);
                    requestDTO.HomeName = homeName;
                    requestDTO.RequestDate = request.CreatedAt;
                    requestDTO.RequestState = request.RequestState;
                    requestDTOs.Add(requestDTO);
                }
                return GenericResult<IReadOnlyList<UserSubscriptionRequestDTO>>.Success(requestDTOs);
            }
            catch (Exception ex)
            {
                _logger.LogCritical("UserInfoService: GetUserNewHomeSubscriptionRequsetsAsync: {error}", ex.Message);
                return GenericResult<IReadOnlyList<UserSubscriptionRequestDTO>>.Failure(ErrorType.Conflict, ex.Message);
            }
        }
    }
}
