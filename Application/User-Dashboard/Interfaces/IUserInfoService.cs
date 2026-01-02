using Application.Common.DTOs;
using Application.GenericResult;
using Application.Home_Management.DTOs;
using Application.User_Dashboard.DTOs;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.User_Dashboard.Interfaces
{
    public interface IUserInfoService
    {
        Task<GenericResult<UserGeneralInfoDTO>> GetUserInfoAsync(Guid userId); // [Done]

        Task<GenericResult<bool>> ChangeUserPhoneNumberAsync(string phoneNumber, Guid userId); // [Done]
        Task<GenericResult<bool>> ChangeUserNameAsync(string newUserName, Guid userId);
        Task<GenericResult<string?>> ChangeUserImageAsync(IFormFile image, Guid userId); // [Done]
        Task<GenericResult<bool>> ChangeUserDisplayNameAsync(string newDisplayName, Guid userId); // [Done]

        Task<GenericResult<IReadOnlyList<UserHomesDTO>>> GetUserHomesAsync(Guid userId); // [Done]

        Task<GenericResult<bool>> SendHomesubscriptionRequestAsync(Guid userId, Guid homeId); // [Done]
        Task<GenericResult<bool>> DeleteHomesubscriptionRequestAsync(Guid requestId); // [Done]

        Task<GenericResult<IReadOnlyList<UserSubscriptionRequestDTO>>> GetUserAllHomeSubscriptionRequsetsAsync(Guid userId); // [Done]
        Task<GenericResult<IReadOnlyList<UserSubscriptionRequestDTO>>> GetUserNewHomeSubscriptionRequsetsAsync(Guid userId); // [Done]
    }
}
