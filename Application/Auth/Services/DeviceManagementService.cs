using Application.Abstractions.Cashing.interfaces;
using Application.Abstractions.Cashing.Models;
using Application.Abstractions.Messaging.DTOs;
using Application.Abstractions.Messaging.EmailBodies;
using Application.Abstractions.Messaging.Interfaces;
using Application.Auth.DTOs;
using Application.Auth.Interfaces;
using Application.Entities.SqlEntities.UsersEntities;
using Application.GenericResult;
using Application.RepositotyInterfaces;
using Application.RuleServices;
using Domain.Entities.SqlEntities.UsersEntities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Auth.Services
{
    public class DeviceManagementService : IDeviceManagementService
    {
        //private readonly IAuthService _auth;
        private readonly IServiceProvider _serviceProvider; // we do that to break the  circular dependency
        private readonly IEmailService _email;
        private readonly IHashingService _hashing;
        private readonly IOtpDeviceCacheStore _otpDeviceCache;
        private readonly IUnitOfWork _work;
        public DeviceManagementService(
            IEmailService email,
            //IAuthService auth,
            IServiceProvider serviceProvider, // we do that to break the  circular dependency
            IUnitOfWork work,
            IHashingService hashing,
            IOtpDeviceCacheStore otpDeviceCache
            )
        {
            _email = email;
            //_auth = auth;
            _serviceProvider = serviceProvider; // we do that to break the  circular dependency
            _work = work;
            _hashing = hashing;
            _otpDeviceCache = otpDeviceCache;
        }

        public async Task<GenericResult<Guid>> SendDeviceCheckingOtpAsync(LoginDTO req, Guid appUserId)
        {
            if (string.IsNullOrEmpty(req.Email) && string.IsNullOrEmpty(req.Username))
                return GenericResult<Guid>.Failure(
                    ErrorType.InvalidData,
                    "Email or Username must be provided to send device checking email.");
            try
            {
                var email = req.Email ?? (await _work.AppUser.GetUserIdentityInfoAsync(appUserId)).Value.email;
                // generate OTP
                var otp = new Random().Next(100000, 999999);
                // hash OTP
                var (otpHash, otpSalt) = _hashing.Hash(otp.ToString());
                // create OTP device challenge cache entry
                var otpDeviceChallenge = OtpDeviceChallengeCache.Create(
                    appUserId,
                    Convert.ToBase64String(otpHash),
                    Convert.ToBase64String(otpSalt),
                    req.DeviceIP,
                    req.DeviceMACAddress,
                    email
                    );
                // save in Redis
                await _otpDeviceCache.SaveAsync(otpDeviceChallenge);
                // create Email Body
                var Email = new EmailDTO
                   (
                   email,
                   "islamahmed920.al@gmail.com",
                   "Device Access Authenticaion Check",
                   OTPEmailBody.OtpCheckingMail(email, otp ) 
                   );
                // send Email
                await _email.SendEmailAsync(Email);
                return GenericResult<Guid>.Success(otpDeviceChallenge.Id, "Device verification code sent successfully, with the questionId"); 
            }
            catch (Exception ex)
            {
                return GenericResult<Guid>.Failure(
                    ErrorType.Conflict,
                    $"An error occurred while sending device verification code: {ex.Message}");
            }
        }

        public async Task<GenericResult<bool>> UpdateDeviceMACAsync(string oldDeviceMAC, string newDeviceMAC)
        {
            if (string.IsNullOrEmpty(oldDeviceMAC) || string.IsNullOrEmpty(newDeviceMAC))
                return GenericResult<bool>.Failure(ErrorType.InvalidData, "Old or new device MAC address is missing.");
            try
            {
                var updateResult = await _work.AppDevice.UpdateAppDeviceMACAsync(oldDeviceMAC, newDeviceMAC);
                if (!updateResult.IsSuccess)
                    return GenericResult<bool>.Failure(updateResult.ErrorType, updateResult.ErrorMessage ?? "Failed to update device MAC address.");
                return GenericResult<bool>.Success(true, "Device MAC address updated successfully.");
            }
            catch (Exception ex)
            {
                return GenericResult<bool>.Failure(ErrorType.Conflict, $"An error occurred while updating device MAC address: {ex.Message}");
            }
        }

        public async Task<GenericResult<AuthResponseDTO>> VerifyOtpAsync(Guid questionId, int otpAnswer ,string deviceMAC)
        {
            if (otpAnswer < 100000 || otpAnswer > 999999)
                return GenericResult<AuthResponseDTO>.Failure(ErrorType.InvalidData,"OTP answer must be a 6-digit number.");     
            if (questionId == Guid.Empty)
                return GenericResult<AuthResponseDTO>.Failure(ErrorType.InvalidData, "Invalid question ID.");   
            try
            {
                var otpChallenge =  await _otpDeviceCache.GetAsync(questionId);
                if (otpChallenge == null)
                    return GenericResult<AuthResponseDTO>.Failure(ErrorType.NotFound, "OTP challenge not found or has expired, try to login again!");
                if (otpChallenge.DeviceMACAddress != deviceMAC)
                    return GenericResult<AuthResponseDTO>.Failure(ErrorType.Unauthorized, "Device has been changed!");
                var isValid = _hashing.Verify(
                    otpAnswer.ToString(),
                    Convert.FromBase64String(otpChallenge.OtpCodeHash),
                    Convert.FromBase64String(otpChallenge.OtpSalt)
                    );
                if (!isValid)
                    return GenericResult<AuthResponseDTO>.Failure(ErrorType.Unauthorized, "Invalid OTP answer.");
                else
                {
                    // check if device already exists
                    var existingDevice = await _work.AppDevice.CheckDeviceExistsAsync(otpChallenge.DeviceMACAddress);
                    AppDevice deviceResult = null!;
                    if (!existingDevice.Value)
                    {
                        // save trusted device info in DB
                        deviceResult = (await _work.AppDevice.AddAppDeviceAsync(
                            otpChallenge.DeviceMACAddress,
                            otpChallenge.DeviceIP,
                            "Default Device Name",
                            "Default Device Type"
                            )).Value!;
                    }
                    else
                    {
                        deviceResult = (await _work.AppDevice.GetAppDeviceByMACAddressAsync(otpChallenge.DeviceMACAddress)).Value!;
                        // Update Device IP
                        if (deviceResult.DeviceIP != otpChallenge.DeviceIP)
                        {
                            await _work.AppDevice.UpdateAppDeviceIPAsync(otpChallenge.DeviceMACAddress, otpChallenge.DeviceIP);
                        }
                    }
                    if (deviceResult == null)
                    {
                        return GenericResult<AuthResponseDTO>.Failure(ErrorType.Conflict, "An error occurred while retrieving or creating the device.");
                    }
                    // Assign Device to User
                    await _work.AppDevice.AssignDeviceToUserAsync(otpChallenge.DeviceMACAddress, otpChallenge.AppUserId);
                    // OTP is valid, remove the challenge from cache
                    await _otpDeviceCache.RemoveAsync(questionId);
                    // Get Tokens
                    // we do that to break the  circular dependency
                    var authService = _serviceProvider.GetRequiredService<IAuthService>();
                    var tokensResult = await authService.IssueTokensAsync(otpChallenge.AppUserId, otpChallenge.Email!, deviceResult.Id);
                    // return success
                    return GenericResult<AuthResponseDTO>.Success(tokensResult, "OTP verified successfully.");
                }
            }
            catch (Exception ex)
            {
                return GenericResult<AuthResponseDTO>.Failure(ErrorType.Conflict,$"An error occurred while verifying OTP: {ex.Message}");    
            }
        }
    
    }
}
