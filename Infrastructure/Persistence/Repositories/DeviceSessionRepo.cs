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
    public class DeviceSessionRepo : IDeviceSessionRepo
    {
        private readonly AppDbContext _context;
        public DeviceSessionRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<GenericResult<AppDevice>> AssignNewTokenToDeviceAsync(string deviceMACAddress, UserRefreshToken token)
        {
            if (string.IsNullOrEmpty(deviceMACAddress))
                return GenericResult<AppDevice>.Failure(ErrorType.MissingData, "Invalid Device MAC Address!");
            if (token == null)
                return GenericResult<AppDevice>.Failure(ErrorType.MissingData, "No Token has been Provided!");
            try
            {
                var device = await _context.AppDevices.FirstOrDefaultAsync(d => d.DeviceMACAddress == deviceMACAddress);
                if (device == null)
                    return GenericResult<AppDevice>.Failure(ErrorType.NotFound, "Device not found!");
                if (_context.UserRefreshTokens.Any(t => t.Id == token.Id))
                    return GenericResult<AppDevice>.Failure(ErrorType.Conflict, "This Token already exists in the System!");
                if (device.RefreshTokens.Any(t => t.Id == token.Id))
                    return GenericResult<AppDevice>.Failure(ErrorType.Conflict, "This Token is already assigned to the Device!");
                device.AddToken(token);
                _context.AppDevices.Update(device);
                var _r = await _context.SaveChangesAsync();
                if (_r <= 0)
                    return GenericResult<AppDevice>.Failure(ErrorType.DatabaseError, "Failed to assign Token to Device!");
                return GenericResult<AppDevice>.Success(device);
            }
            catch (Exception ex)
            {
                return GenericResult<AppDevice>.Failure(ErrorType.Conflict, ex.Message);
            }
        }

        public async Task<GenericResult<List<UserRefreshToken>>> GetAllActiveTokensOfDeviceAsync(string deviceMACAddress)
        {
            if (string.IsNullOrEmpty(deviceMACAddress))
                return GenericResult<List<UserRefreshToken>>.Failure(ErrorType.MissingData, "Invalid Device MAC Address!");
            try
            {
                var deviceToken = await _context.AppDevices
                    .AsNoTracking()
                    .Include(AD => AD.RefreshTokens)
                    .FirstOrDefaultAsync(d => d.DeviceMACAddress == deviceMACAddress);

                if (deviceToken == null)
                    return GenericResult<List<UserRefreshToken>>.Failure(ErrorType.NotFound, "Device not found!");

                var activeTokens = deviceToken.RefreshTokens.Where(t => (!(t.Revoked)&&(t.ExpiresAt >= DateTime.UtcNow)))
                    .OrderBy(t => t.CreatedAt).ToList();

                if(activeTokens is null)
                    return GenericResult<List<UserRefreshToken>>.Failure(ErrorType.NotFound, "Device not found!");
                return GenericResult<List<UserRefreshToken>>.Success(activeTokens);
            }
            catch (Exception ex)
            {
                return GenericResult<List<UserRefreshToken>>.Failure(ErrorType.Conflict, ex.Message);
            }
        }
           
        public async Task<GenericResult<List<UserRefreshToken>>> GetAllTokensOfDeviceAsync(string deviceMACAddress)
            => GenericResult<List<UserRefreshToken>>.Success(
                await _context.AppDevices
                .AsNoTracking()
                .Where(d => d.DeviceMACAddress == deviceMACAddress)
                .SelectMany(d => d.RefreshTokens)
                .ToListAsync()
            );

        public async Task<GenericResult<AppDevice>> RemoveTokenFromDeviceAsync(string deviceMACAddress, Guid tokenId)
        {
            if (string.IsNullOrEmpty(deviceMACAddress))
                return GenericResult<AppDevice>.Failure(ErrorType.MissingData, "Invalid Device MAC Address!");
            if (tokenId == Guid.Empty)
                return GenericResult<AppDevice>.Failure(ErrorType.MissingData, "Invalid Token Id!");
            try
            {
                var device = await _context.AppDevices
                    .Include(d => d.RefreshTokens)
                    .FirstOrDefaultAsync(d => d.DeviceMACAddress == deviceMACAddress);
                if (device == null)
                    return GenericResult<AppDevice>.Failure(ErrorType.NotFound, "Device not found!");
                var token = device.RefreshTokens.FirstOrDefault(t => t.Id == tokenId);
                if (token == null)
                    return GenericResult<AppDevice>.Failure(ErrorType.NotFound, "Token not found on the Device!");
                _context.UserRefreshTokens.Remove(token);
                var _r = await _context.SaveChangesAsync();
                if (_r <= 0)
                    return GenericResult<AppDevice>.Failure(ErrorType.DatabaseError, "Failed to remove Token from Device!");
                return GenericResult<AppDevice>.Success(device);
            }
            catch (Exception ex)
            {
                return GenericResult<AppDevice>.Failure(ErrorType.Conflict, ex.Message);
            }
        }
    }
}
