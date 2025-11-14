using Application.Abstractions.Identity;
using Application.Abstractions.Identity.DTOs;
using Application.Abstractions.Messaging.DTOs;
using Application.Abstractions.Messaging.EmailBodies;
using Application.Abstractions.Messaging.Interfaces;
using Application.Abstractions.Security.Interfaces;
using Application.Abstractions.Time;
using Application.Auth.DTOs;
using Application.Auth.Interfaces;
using Application.Entities.SqlEntities.UsersEntities;
using Application.GenericResult;
using Application.RepositotyInterfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using Application.RuleServices;
using Microsoft.Extensions.Configuration;

namespace Application.Auth.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly ICustomTokenService _customTokens;
        private readonly IJwtTokenService _jwtTokens;
        private readonly IHashingService _hashing;
        private readonly IIdentityManagement _users;
        private readonly IDateTimeProvider _clock;
        private readonly IEmailService _email;
        private readonly IUnitOfWork _work;
        private readonly IDeviceCheckingService _DeviceCheck;

        public AuthService(
            IIdentityManagement users,
            ICustomTokenService customTokens,
            IHashingService hashing,
            IJwtTokenService jwtTokens,
            IDateTimeProvider clock,
            IEmailService email,
            IUnitOfWork work,
            IConfiguration configuration,
            IDeviceCheckingService deviceCheck)
        {
            _users = users;
            _customTokens = customTokens;
            _jwtTokens = jwtTokens;
            _hashing = hashing;
            _clock = clock;
            _email = email;
            _work = work;
            _configuration = configuration;
            _DeviceCheck = deviceCheck;
        }

        private int RTExpriresInDays = 7;

        private async Task SendActivationEmail(string email) // [Done]
        {
            string activationToken = await _users.GenerateEmailConfirmationTokenAsync(email);
            var activationUrl = _configuration["FronEndInfo:FronEndUrl:ActivationUrl"]!;
            var activationComponent = _configuration["FronEndInfo:FronEndComponent:ActivationComponent"]!;
            var shortMessage = "Activate";
            var emailMessage = "Activate your account by clicking the link below:";

            var Email = new EmailDTO
               (
               email,
               "islamahmed920.al@gmail.com",
               "Email Activation",
               RedirectionEmailBody.RedirectionMail(
                   activationUrl, email, emailMessage,
                   activationToken, activationComponent, shortMessage
                   )
               );

             await _email.SendEmailAsync(Email);
        }

        private async Task SendResetPasswordEmail(string email) // [Done]
        {
            string resetPWToken = await _users.GeneratePasswordResetTokenAsync(email);
            var resetPWUrl = _configuration["FronEndInfo:FronEndUrl:ResetPasswordUrl"]!;
            var resetPWComponent = _configuration["FronEndInfo:FronEndComponent:ResetPasswordComponent"]!;
            var shortMessage = "Password reset";
            var emailMessage = "Click on the button to reset your password";

            var Email = new EmailDTO
               (
               email,
               "islamahmed920.al@gmail.com",
               "Reset Password request",
               RedirectionEmailBody.RedirectionMail(
                   resetPWUrl, email, emailMessage,
                   resetPWToken, resetPWComponent, shortMessage)
               );

            await _email.SendEmailAsync(Email);
        }

        private async Task<AuthResponseDTO?> IssueTokensAsync(UserIdentityDTO user, Guid deviceId) // [Done]
        {
            var appUserId = await _users.GetAppUserIdAsync(user.Id);
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, appUserId.ToString()), // Use AppUserId as NameIdentifier, not IdentityUserId for security.
                new(ClaimTypes.Name, user.UserName ?? ""),
                new(ClaimTypes.Email, user.Email ?? ""),
                new(ClaimTypes.WindowsDeviceClaim,deviceId.ToString())// Device info claim can be added here if needed
            };
            // create Access jwt token 
            var (access, expires) = _jwtTokens.GenerateAccessToken(claims);
            // create Refresh token
            var refresh = _customTokens.GenerateRefreshToken(); 
            // Generate the Hash and Salt for the refresh token
            var (refreshHash, refreshSalt) = _hashing.Hash(refresh);
            // save the refresh token in the DataBase
            var refreshTokenEntity = _work.UserRefreshToken.SaveTokenAsync(
                appUserId,
                Convert.ToBase64String(refreshSalt),
                Convert.ToBase64String(refreshHash),
                RTExpriresInDays,
                deviceId
                );
            if (access == null || refresh == null)
                return null;
            return new AuthResponseDTO { AccessToken = access, RefreshToken = refresh, ExpiresAtUtc = expires };
        }

        public async Task<AuthResponseDTO?> IssueTokensAsync(Guid userId, string email, Guid deviceId) // [Done]
        {
            var appUser = await _work.AppUser.GetUserInfoAsync(userId);
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userId.ToString()), // Use AppUserId as NameIdentifier, not IdentityUserId for security.
                new(ClaimTypes.Name, appUser.Value.userName ),
                new(ClaimTypes.Email, email),
                new(ClaimTypes.WindowsDeviceClaim,deviceId.ToString())// Device info claim can be added here if needed
            };
            // create Access jwt token 
            var (access, expires) = _jwtTokens.GenerateAccessToken(claims);
            // create Refresh token
            var refresh = _customTokens.GenerateRefreshToken();
            // Generate the Hash and Salt for the refresh token
            var (refreshHash, refreshSalt) = _hashing.Hash(refresh);
            // save the refresh token in the DataBase
            var refreshTokenEntity = _work.UserRefreshToken.SaveTokenAsync(
                userId,
                Convert.ToBase64String(refreshSalt),
                Convert.ToBase64String(refreshHash),
                RTExpriresInDays,
                deviceId
                );
            if (access == null || refresh == null)
                return null;
            return new AuthResponseDTO { AccessToken = access, RefreshToken = refresh, ExpiresAtUtc = expires };
        }

        public async Task<GenericResult<string>> RegisterAsync(RegisterRequestDTO req) // [Done]
        {
            if (req == null)
                return GenericResult<string>.Failure(ErrorType.NullableValue, "There Is No Data Have been Sent");
            try
            {
                if (await _users.FindByNameAsync(req.Username) is not null)
                    return GenericResult<string>.Failure(ErrorType.MissingData, "this UserName can not be used! ");
                if (await _users.FindByEmailAsync(req.Email) is not null)
                    return GenericResult<string>.Failure(ErrorType.MissingData, "this Email can not be used! ");

                var registrationResult = await _users.CreateAsync(req.Username,req.Email,req.Password,req.DisplayName);  
                if (registrationResult == null)
                {
                    return GenericResult<string>.Failure(ErrorType.Conflict, registrationResult!.Errors);
                }
                else
                {
                    // send active Email:
                    await SendActivationEmail(req.Email);
                    return GenericResult<string>.Success("Done", "Account has Successfully Registered");
                }
            }
            catch (Exception ex) { return GenericResult<string>.Failure(ErrorType.Conflict, ex.Message); }
        }

        public async Task<GenericResult<bool>> AccountActivation(AccountActivationDTO accountActivationDTO) // [Done]
        {
            if (accountActivationDTO == null)
                return GenericResult<bool>.Failure(ErrorType.NullableValue, "the input parameter is null");
            try
            {
                var findUser = await _users.FindByEmailAsync(accountActivationDTO.UserEmail);
                if (findUser is null)
                    return GenericResult<bool>.Failure(ErrorType.NotFound, "There is no Account with this Email");

                var confirm = await _users.ConfirmEmailAsync(findUser.Email, accountActivationDTO.ActivationToken);
                if (confirm)
                {
                    // create AppUser Entity Here 
                    var appUser = AppUser.Create(findUser.Id);
                    var createResult = await _work.AppUser.AddUserAsync(appUser);
                    // Check if the Device is already exists:
                    if (!(await _work.AppDevice.CheckDeviceExistsAsync(accountActivationDTO.DeviceMACAddress)).Value)
                    {
                        // Create the Device and assign it to the user
                        var deviceResult = await _work.AppDevice.AddAppDeviceAsync(
                            accountActivationDTO.DeviceMACAddress,
                            accountActivationDTO.DeviceIP,
                            "Default Device Name",
                            "Default Device Type"
                            );
                        if (!(deviceResult.IsSuccess && deviceResult.Value != null))
                        {
                            // return success
                            return GenericResult<bool>.Success(true, "Account has been Activated, But the Device is still not been Authorized!");
                        }        
                    }
                    // Assign Device to User
                    await _work.AppDevice.AssignDeviceToUserAsync(accountActivationDTO.DeviceMACAddress, appUser.Id);
                    // return success
                    return GenericResult<bool>.Success(true, "Account has been Activated");
                }
                else
                {
                    // send active Email:
                    await SendActivationEmail(findUser.Email);
                    return GenericResult<bool>.Failure(ErrorType.NotFound, "\tActivation Denaied,\nActivation Email has been sent agein");
                }
            }
            catch (Exception ex) { return GenericResult<bool>.Failure(ErrorType.Conflict, ex.Message); }
        }

        public async Task<GenericResult<string>> SendEmailForForgottenPassword(string email) // [Done]
        {
            if (email == null)
                return GenericResult<string>.Failure(ErrorType.NullableValue, "the input parameter is null");
            try
            {
                var findUser = await _users.FindByEmailAsync(email);
                if (findUser is null)
                    return GenericResult<string>.Failure(ErrorType.NotFound, "There is no Account with this Email");

                await SendResetPasswordEmail(findUser.Email);

                return GenericResult<string>.Success("Done");
            }
            catch (Exception ex) { return GenericResult<string>.Failure(ErrorType.Conflict, ex.Message); }
        }

        public async Task<GenericResult<string>> ResetPassword(ResetPasswordDTO rpw) // [work just with foget password]
        {
            if (rpw == null)
                return GenericResult<string>.Failure(ErrorType.NullableValue, "the input parameter is null");
            try
            {
                var findUser = await _users.FindByEmailAsync(rpw.UserEmail);
                if (findUser is null)
                    return GenericResult<string>.Failure(ErrorType.NotFound, "There is no Account with this Email");

                var resetPwResult = await _users.ResetPasswordAsync(rpw.UserEmail, rpw.ResetPWToken, rpw.UserPassword);

                if (resetPwResult != null)
                    return GenericResult<string>.Success("Done", "Password Has Changed");

                return GenericResult<string>.Failure(ErrorType.Conflict, resetPwResult);
            }
            catch (Exception ex) { return GenericResult<string>.Failure(ErrorType.Conflict, ex.Message); }
        }

        public async Task<GenericResult<AuthResponseDTO>> LoginAsync(LoginRequestDTO req) // [Done]
        {
            if (req == null)
                return GenericResult<AuthResponseDTO>.Failure(ErrorType.NullableValue);
            try
            {
                // 1[] check if the user use the UserName or Email:
                UserIdentityDTO? findUser = null;
                if (req.Email == null)
                    findUser = await _users.FindByEmailAsync(req.Email!);
                else
                    findUser = await _users.FindByNameAsync(req.Username!);

                if (findUser != null)
                {
                    // 2[] the Account is confirmed
                    if (!await _users.CheckEmailConfirmedAsync( req.Email ?? req.Username))
                    {
                        await SendActivationEmail(findUser.Email);
                        return GenericResult<AuthResponseDTO>.Failure(ErrorType.InvalidData
                            , "Please Confirm Your Account First !\n There is a new Activation Mail has been sent to you");
                    }
                   
                    // 3[] Check the Password:
                    var check = await _users.CheckPasswordSignInAsync(findUser.Email,req.Password);
                    if (!check)
                        return GenericResult<AuthResponseDTO>.Failure(ErrorType.Validation, "The Password is Wrong");
                    // Get the AppUserId for the user:
                    var appUserId = await _users.GetAppUserIdAsync(findUser.Id);
                    // 4[] Check if the login done from a known Device "MACAddress" (Optional)-> 
                    if (!(await _work.AppDevice.CheckDeviceAssignedToUserAsync(req.DeviceMACAddress,appUserId)).Value)
                    {
                        // Device is not assigned to this user
                        // Send a Checking Mail to the user by using DeviceCheckingService 
                        await _DeviceCheck.SendDeviceCheckingOtpAsync(req, appUserId); 
                        return GenericResult<AuthResponseDTO>.Failure(ErrorType.Unauthorized, "The Used Device is not Authorized!");
                    }
                    else
                    {
                        // Update Device IP if needed
                        if(!(await _work.AppDevice.CheckDeviceIPAsync(req.DeviceMACAddress,req.DeviceIP)).Value)
                        {
                            await _work.AppDevice.UpdateAppDeviceIPAsync(req.DeviceMACAddress, req.DeviceIP);
                        }
                        // Get the DeviceId
                        var deviceIdResult = await _work.AppDevice.GetDeviceIdByMACAddressAsync(req.DeviceMACAddress);
                        // Finally[] Get the Tokens:
                        var returnValue = await IssueTokensAsync(findUser!, deviceIdResult.Value);
                        // check if the Token Generation is successfull:
                        if (returnValue == null)
                            return GenericResult<AuthResponseDTO>.Failure(ErrorType.Conflict, "Token Generation Failed");
                        return GenericResult<AuthResponseDTO>.Success(returnValue, "Login Successful");
                    }
                }
                return GenericResult<AuthResponseDTO>.Failure(ErrorType.NotFound, "There is no User with this Data!");
            }
            catch (Exception ex) { return GenericResult<AuthResponseDTO>.Failure(ErrorType.Conflict, ex.Message); }
        }

        public async Task<GenericResult<AuthResponseDTO>> RefreshAsync(string refreshToken, string deviceMAC) // [Done]
        {
            if (string.IsNullOrWhiteSpace(refreshToken) || string.IsNullOrWhiteSpace(deviceMAC))
                return GenericResult<AuthResponseDTO>.Failure(ErrorType.NullableValue, "Missing data!.");
            try
            {
                // 1[] Get the Device Active Tokens by MAC Address
                var deviceTokensResult = await _work.DeviceSession.GetAllActiveTokensOfDeviceAsync(deviceMAC);
                if (deviceTokensResult.Value!.Count != 1)
                    return GenericResult<AuthResponseDTO>.Failure(ErrorType.Unauthorized, "The Device is not Authorized!");

                // Get the Active Token
                var oldActiveToken = deviceTokensResult.Value[0];

                // 2[] Check the Only Actice Token
                var isValid = _hashing.Verify(
                    refreshToken,
                    Convert.FromBase64String(oldActiveToken.TokenHash),
                    Convert.FromBase64String(oldActiveToken.TokenSalt)
                    );

                // If The Token isn't Valid
                if (isValid == false)
                    return GenericResult<AuthResponseDTO>.Failure(ErrorType.Unauthorized, "Invalid Token!");

                // If The Token is Valid
                // 1- create new tokens
                var userInfo = await _work.AppUser.GetUserInfoAsync(oldActiveToken.AppUserId);
                var tokens = await IssueTokensAsync(oldActiveToken.AppUserId, userInfo.Value.email, oldActiveToken.DeviceId);
                // 2- Hash the new created token
                var refreshTokenHashData = _hashing.Hash(tokens!.RefreshToken);
                // 3- Rotate the Refresh Tokens, and create new UserRefreshToken (Rotate method aready create new token).
                var newTKId = await _work.UserRefreshToken.RotateRT_DBProcessAsync(
                    oldActiveToken.Id,
                    oldActiveToken.AppUserId,
                    Convert.ToBase64String(refreshTokenHashData.Salt),
                    Convert.ToBase64String(refreshTokenHashData.Hash),
                    RTExpriresInDays);

                if (newTKId.IsSuccess)
                    return GenericResult<AuthResponseDTO>.Success(tokens, "Refresh Request has been Done!");

                return GenericResult<AuthResponseDTO>.Failure(ErrorType.Conflict, "Can not create New Tokens!");
            }
            catch (Exception ex)
            {
                return GenericResult<AuthResponseDTO>.Failure(ErrorType.Conflict, ex.Message);
            }
        }

        public async Task<GenericResult<bool>> DeleteAccount(string email) // [Done]
        {
            if (string.IsNullOrWhiteSpace(email))
                return GenericResult<bool>.Failure(ErrorType.Validation, "Email must be provided.");
            try
            {
                var finduser = await _users.FindByEmailAsync(email);
                if (finduser != null)
                {
                    var deletingResult = await _users.DeleteAsync(email);
                    if (deletingResult)
                    {
                        // Also, consider deleting related AppUser and AppDevice entities if needed
                        var appUserId = await _users.GetAppUserIdAsync(finduser.Id);
                        // Delete AppUser from the Devices first to avoid FK constraint issues
                        await _work.AppDevice.RemoveUserFromAllDevicesAsync(appUserId);
                        // Then delete the AppUser
                        await _work.AppUser.RemoveUserAsync(appUserId);
                        // Return success
                        return GenericResult<bool>.Success(true, "the Account Has been deleted Successfully!"); 
                    }
                    return GenericResult<bool>.Failure(ErrorType.Conflict, "User can not be deleted!");
                }
                else
                    return GenericResult<bool>.Failure(ErrorType.Validation, "this UserName or the Email is not correct !");
            }
            catch (Exception ex) { return GenericResult<bool>.Failure(ErrorType.Conflict, ex.Message); }
        }

        public async Task<GenericResult<bool>> LogoutAsync(LogoutDTO req) // [Done]
        {
            if (req == null)
                return GenericResult<bool>.Failure(ErrorType.NullableValue, "There Is No Data Have been Sent");
            try
            {
                // 1- Get the Device Active Tokens by MAC Address
                var deviceTokensResult = await _work.DeviceSession.GetAllActiveTokensOfDeviceAsync(req.DeviceMAC);
                if (deviceTokensResult.Value!.Count != 1)
                    return GenericResult<bool>.Failure(ErrorType.Unauthorized, "The Device is not Authorized!");
                // Get the Active Token
                var oldActiveToken = deviceTokensResult.Value[0];
                // 2- Check the Only Actice Token
                var isValid = _hashing.Verify(
                    req.RefreshToken,
                    Convert.FromBase64String(oldActiveToken.TokenHash),
                    Convert.FromBase64String(oldActiveToken.TokenSalt)
                    );
                // If The Token isn't Valid
                if (isValid == false)
                    return GenericResult<bool>.Failure(ErrorType.Unauthorized, "Invalid Token!");
                // If The Token is Valid
                var logoutResult = await _work.UserRefreshToken.RevokeTokenAsync(oldActiveToken.Id);
                if (logoutResult.IsSuccess)
                    return GenericResult<bool>.Success(true, "Logout Successful");
                return GenericResult<bool>.Failure(ErrorType.Conflict, "Logout Failed");
            }
            catch (Exception ex)
            {
                return GenericResult<bool>.Failure(ErrorType.Conflict, ex.Message);
            }
        }
    }
}
