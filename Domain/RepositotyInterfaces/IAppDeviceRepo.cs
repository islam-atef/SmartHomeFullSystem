using Application.Entities.SqlEntities.SecurityEntities;
using Application.GenericResult;
using Domain.Entities.SqlEntities.UsersEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.RepositotyInterfaces
{
    public interface IAppDeviceRepo
    {
        Task<GenericResult<AppDevice>> AddAppDeviceAsync(string deviceMACAddress, string deviceIP, string name, string type);

        Task<GenericResult<List<AppDevice>>> GetAllDevicesAsync();
        Task<GenericResult<Guid>> GetDeviceIdByMACAddressAsync(string deviceMACAddress);
        Task<GenericResult<List<AppDevice>>> GetAllDevicesOfUserAsync(Guid appUserId);
        Task<GenericResult<AppDevice>> GetAppDeviceByMACAddressAsync(string deviceMACAddress);
        Task<GenericResult<AppDevice>> GetAppDeviceByIdAsync(Guid appDeviceId);

        Task<GenericResult<AppDevice>> UpdateAppDeviceIPAsync(string deviceMACAddress, string deviceIP);
        Task<GenericResult<AppDevice>> UpdateAppDeviceNameAsync(string deviceMACAddress, string name);

        Task<GenericResult<AppDevice>> AssignDeviceToUserAsync(string deviceMACAddress, Guid appUserId);

        Task<GenericResult<bool>> CheckDeviceExistsAsync(string deviceMACAddress);
        Task<GenericResult<bool>> CheckDeviceAssignedToUserAsync(string deviceMACAddress, Guid appUserId);
        Task<GenericResult<bool>> CheckDeviceIPAsync(string deviceMACAddress, string deviceIP);

        Task<GenericResult<bool>> RemoveAppDeviceByMACAddressAsync(string deviceMACAddress);
        Task<GenericResult<bool>> RemoveUserFromDeviceAsync(string deviceMACAddress, Guid appUserId);
        Task<GenericResult<bool>> RemoveUserFromAllDevicesAsync(Guid appUserId);
    }
}
