using Domain.Entities.SqlEntities.SecurityEntities;
using Domain.GenericResult;
using Domain.Entities.SqlEntities.UsersEntities;
using Domain.RepositotyInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class AppDeviceRepo : IAppDeviceRepo
    {
        private readonly AppDbContext _context;
        public AppDeviceRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<GenericResult<AppDevice>> AddAppDeviceAsync(string deviceMACAddress, string deviceIP, string name, string type)
        {
            if (string.IsNullOrEmpty(deviceMACAddress))
                return GenericResult<AppDevice>.Failure(ErrorType.MissingData, "Invalid Device MAC Address!");
            if (string.IsNullOrEmpty(deviceIP) || String.IsNullOrEmpty(type) || String.IsNullOrEmpty(name))
                return GenericResult<AppDevice>.Failure(ErrorType.MissingData, "Missing Data!");
            try
            {
                if (await _context.AppDevices.AnyAsync(d => d.DeviceMACAddress == deviceMACAddress))
                    return GenericResult<AppDevice>.Failure(ErrorType.Conflict, "Device with the same MAC Address already exists!");
                var newDevice = AppDevice.Create(deviceMACAddress, deviceIP, name, type);
                await _context.AppDevices.AddAsync(newDevice);
                var _r = await _context.SaveChangesAsync();
                if (_r <= 0)
                    return GenericResult<AppDevice>.Failure(ErrorType.DatabaseError, "Failed to add new Device!");
                return GenericResult<AppDevice>.Success(newDevice);
            }
            catch (Exception ex)
            {
                return GenericResult<AppDevice>.Failure(ErrorType.Conflict, ex.Message);
            }
        }

        public async Task<GenericResult<AppDevice>> AssignDeviceToUserAsync(string deviceMACAddress, Guid appUserId)
        {
            if (string.IsNullOrEmpty(deviceMACAddress))
                return GenericResult<AppDevice>.Failure(ErrorType.MissingData, "Invalid Device MAC Address!");
            if (appUserId == Guid.Empty)
                return GenericResult<AppDevice>.Failure(ErrorType.MissingData, "Invalid App User Id!");
            try
            {
                var device = await _context.AppDevices
                    .Include(d => d.DeviceUsers)
                    .FirstOrDefaultAsync(d => d.DeviceMACAddress == deviceMACAddress);
                if (device == null)
                    return GenericResult<AppDevice>.Failure(ErrorType.NotFound, "Device not found!");
                var user = await _context.AppUsers.FirstOrDefaultAsync(u => u.Id == appUserId);
                if (user == null)
                    return GenericResult<AppDevice>.Failure(ErrorType.NotFound, "User not found!");
                if (device.DeviceUsers.Any(u => u.Id == appUserId))
                    return GenericResult<AppDevice>.Failure(ErrorType.Conflict, "Device is already assigned to this user!");
                device.AddUser(user);
                _context.AppDevices.Update(device);
                var _r = await _context.SaveChangesAsync();
                if (_r <= 0)
                    return GenericResult<AppDevice>.Failure(ErrorType.DatabaseError, "Failed to assign Device to User!");
                return GenericResult<AppDevice>.Success(device);
            }
            catch (Exception ex)
            {
                return GenericResult<AppDevice>.Failure(ErrorType.Conflict, ex.Message);
            }
        }

        public async Task<GenericResult<bool>> CheckDeviceAssignedToUserAsync(string deviceMACAddress, Guid appUserId)
            => GenericResult<bool>.Success(
                await _context.AppDevices
                    .AsNoTracking()
                    .AnyAsync(d => d.DeviceMACAddress == deviceMACAddress && d.DeviceUsers.Any(u => u.Id == appUserId))
            );

        public async Task<GenericResult<bool>> CheckDeviceExistsAsync(string deviceMACAddress)
            => GenericResult<bool>.Success(
                await _context.AppDevices
                    .AsNoTracking()
                    .AnyAsync(d => d.DeviceMACAddress == deviceMACAddress)
            );

        public async Task<GenericResult<bool>> CheckDeviceIPAsync(string deviceMACAddress, string deviceIP)
            => GenericResult<bool>.Success(
                await _context.AppDevices
                    .AsNoTracking()
                    .AnyAsync(d => d.DeviceMACAddress == deviceMACAddress && d.DeviceIP == deviceIP)
            );

        public async Task<GenericResult<List<AppDevice>>> GetAllDevicesAsync()
            => GenericResult<List<AppDevice>>.Success(
                await _context.AppDevices
                    .AsNoTracking()
                    .Include(d => d.RefreshTokens).Include(d => d.DeviceUsers)
                    .ToListAsync()
            );

        public async Task<GenericResult<List<AppDevice>>> GetAllDevicesOfUserAsync(Guid appUserId)
            => GenericResult<List<AppDevice>>.Success(
                await _context.AppDevices
                    .AsNoTracking()
                    .Where(d => d.DeviceUsers.Any(u => u.Id == appUserId))
                    .Include(d => d.RefreshTokens)
                    .ToListAsync()
            );

        public async Task<GenericResult<AppDevice>> GetAppDeviceByIdAsync(Guid appDeviceId)
            => GenericResult<AppDevice>.Success(
                await _context.AppDevices
                    .AsNoTracking()
                    .Include(d => d.RefreshTokens).Include(d => d.DeviceUsers)
                    .FirstOrDefaultAsync(d => d.Id == appDeviceId)
            );

        public async Task<GenericResult<AppDevice>> GetAppDeviceByMACAddressAsync(string deviceMACAddress)
            => GenericResult<AppDevice>.Success(
                await _context.AppDevices
                    .AsNoTracking()
                    .Include(d => d.RefreshTokens).Include(d => d.DeviceUsers)
                    .FirstOrDefaultAsync(d => d.DeviceMACAddress == deviceMACAddress)
            );

        public async Task<GenericResult<Guid>> GetDeviceIdByMACAddressAsync(string deviceMACAddress)
             => GenericResult<Guid>.Success(
                await _context.AppDevices
                    .AsNoTracking()
                    .Where(d => d.DeviceMACAddress == deviceMACAddress).Select(d => d.Id).SingleAsync()
             );

        public async Task<GenericResult<bool>> RemoveAppDeviceByMACAddressAsync(string deviceMACAddress)
        {
            if (string.IsNullOrEmpty(deviceMACAddress))
                return GenericResult<bool>.Failure(ErrorType.MissingData, "Invalid Device MAC Address!");
            try
            {
                var device = await _context.AppDevices.FirstOrDefaultAsync(d => d.DeviceMACAddress == deviceMACAddress);
                if (device == null)
                    return GenericResult<bool>.Failure(ErrorType.NotFound, "Device not found!");
                _context.AppDevices.Remove(device);
                var _r = await _context.SaveChangesAsync();
                if (_r <= 0)
                    return GenericResult<bool>.Failure(ErrorType.DatabaseError, "Failed to remove Device!");
                return GenericResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return GenericResult<bool>.Failure(ErrorType.Conflict, ex.Message);
            }
        }

        public async Task<GenericResult<bool>> RemoveUserFromAllDevicesAsync(Guid appUserId)
        {
            if (appUserId == Guid.Empty)
                return GenericResult<bool>.Failure(ErrorType.MissingData, "Invalid App User Id!");
            try
            {
                var devices = await _context.AppDevices
                    .Include(d => d.DeviceUsers)
                    .Where(d => d.DeviceUsers.Any(u => u.Id == appUserId))
                    .ToListAsync();
                foreach (var device in devices)
                {
                    var user = device.DeviceUsers.FirstOrDefault(u => u.Id == appUserId);
                    if (user != null)
                    {
                        device.DeviceUsers.ToList().Remove(user);
                        _context.AppDevices.Update(device);
                    }
                }
                var _r = await _context.SaveChangesAsync();
                if (_r <= 0)
                    return GenericResult<bool>.Failure(ErrorType.DatabaseError, "Failed to remove User from Devices!");
                return GenericResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return GenericResult<bool>.Failure(ErrorType.Conflict, ex.Message);
            }
        }

        public async Task<GenericResult<bool>> RemoveUserFromDeviceAsync(string deviceMACAddress, Guid appUserId)
        {
            if (string.IsNullOrEmpty(deviceMACAddress))
                return GenericResult<bool>.Failure(ErrorType.MissingData, "Invalid Device MAC Address!");
            if (appUserId == Guid.Empty)
                return GenericResult<bool>.Failure(ErrorType.MissingData, "Invalid App User Id!");
            try
            {
                var device = await _context.AppDevices
                    .Include(d => d.DeviceUsers)
                    .FirstOrDefaultAsync(d => d.DeviceMACAddress == deviceMACAddress);
                if (device == null)
                    return GenericResult<bool>.Failure(ErrorType.NotFound, "Device not found!");
                var user = device.DeviceUsers.FirstOrDefault(u => u.Id == appUserId);
                if (user == null)
                    return GenericResult<bool>.Failure(ErrorType.NotFound, "User not assigned to this Device!");
                device.DeviceUsers.ToList().Remove(user);
                _context.AppDevices.Update(device);
                var _r = await _context.SaveChangesAsync();
                if (_r <= 0)
                    return GenericResult<bool>.Failure(ErrorType.DatabaseError, "Failed to remove User from Device!");
                return GenericResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return GenericResult<bool>.Failure(ErrorType.Conflict, ex.Message);
            }
        }

        public async Task<GenericResult<AppDevice>> UpdateAppDeviceIPAsync(string deviceMACAddress, string deviceIP)
        {
            if (string.IsNullOrEmpty(deviceMACAddress))
                return GenericResult<AppDevice>.Failure(ErrorType.MissingData, "Invalid Device MAC Address!");
            if (string.IsNullOrEmpty(deviceIP))
                return GenericResult<AppDevice>.Failure(ErrorType.MissingData, "Invalid Device IP Address!");
            try
            {
                var device = await _context.AppDevices.FirstOrDefaultAsync(d => d.DeviceMACAddress == deviceMACAddress);
                if (device == null)
                    return GenericResult<AppDevice>.Failure(ErrorType.NotFound, "Device not found!");
                device.UpdateIP(deviceIP);
                _context.AppDevices.Update(device);
                var _r = await _context.SaveChangesAsync();
                if (_r <= 0)
                    return GenericResult<AppDevice>.Failure(ErrorType.DatabaseError, "Failed to update Device IP!");
                return GenericResult<AppDevice>.Success(device);
            }
            catch (Exception ex)
            {
                return GenericResult<AppDevice>.Failure(ErrorType.Conflict, ex.Message);
            }

        }

        public async Task<GenericResult<AppDevice>> UpdateAppDeviceMACAsync(string oldMACAddress, string newMACAddress)
        {
            if (string.IsNullOrEmpty(oldMACAddress))
                return GenericResult<AppDevice>.Failure(ErrorType.MissingData, "Invalid Device MAC Old Address!");
            if (string.IsNullOrEmpty(newMACAddress))
                return GenericResult<AppDevice>.Failure(ErrorType.MissingData, "Invalid Device New Address!");
            try
            {
                var device = await _context.AppDevices.FirstOrDefaultAsync(d => d.DeviceMACAddress == oldMACAddress);
                if (device == null)
                    return GenericResult<AppDevice>.Failure(ErrorType.NotFound, "Device not found!");
                device.UpdateMAC(newMACAddress);
                _context.AppDevices.Update(device);
                var _r = await _context.SaveChangesAsync();
                if (_r <= 0)
                    return GenericResult<AppDevice>.Failure(ErrorType.DatabaseError, "Failed to update Device IP!");
                return GenericResult<AppDevice>.Success(device);
            }
            catch (Exception ex)
            {
                return GenericResult<AppDevice>.Failure(ErrorType.Conflict, ex.Message);
            }

        }

        public async Task<GenericResult<AppDevice>> UpdateAppDeviceNameAsync(string deviceMACAddress, string name)
        {
            if (string.IsNullOrEmpty(deviceMACAddress))
                return GenericResult<AppDevice>.Failure(ErrorType.MissingData, "Invalid Device MAC Address!");
            if (string.IsNullOrEmpty(name))
                return GenericResult<AppDevice>.Failure(ErrorType.MissingData, "No Name Provided!");
            try
            {
                var device = await _context.AppDevices.FirstOrDefaultAsync(d => d.DeviceMACAddress == deviceMACAddress);
                if (device == null)
                    return GenericResult<AppDevice>.Failure(ErrorType.NotFound, "Device not found!");
                device.DeviceName = name;
                device.UpdateAudit();
                _context.AppDevices.Update(device);
                var _r = await _context.SaveChangesAsync();
                if (_r <= 0)
                    return GenericResult<AppDevice>.Failure(ErrorType.DatabaseError, "Failed to update Device Name!");
                return GenericResult<AppDevice>.Success(device);
            }
            catch (Exception ex)
            {
                return GenericResult<AppDevice>.Failure(ErrorType.Conflict, ex.Message);
            }
        }
    }
}
