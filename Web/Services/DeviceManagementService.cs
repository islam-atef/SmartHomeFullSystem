using Web.Core.Auth;
using Web.Core.UserDevice;
using Web.Core.Utile;
using Web.Identity;
using Web.Models.AuthModel;
using Web.Models.UserDeviceModel;

namespace Web.Services
{
    public class DeviceManagementService
    {
        private readonly IUserDeviceApi _userDeviceApi;
        private readonly ITokenStore _tokenStore;
        private readonly IDeviceIdentityStore _deviceIdStore;
        private readonly AppAuthenticationStateProvider _authStateProvider;
        public DeviceManagementService(IUserDeviceApi userDeviceApi, ITokenStore tokenStore, AppAuthenticationStateProvider authStateProvider, IDeviceIdentityStore deviceIdStore)
        {
            _userDeviceApi = userDeviceApi;
            _tokenStore = tokenStore;
            _authStateProvider = authStateProvider;
            _deviceIdStore = deviceIdStore;
        }

        public async Task<bool?> VerifyDeviceAsync(Guid? questionId, int otpAnswer)
        {
            try
            {
                // check the input parameters
                if (questionId == Guid.Empty || otpAnswer == 0)
                    return false;
                // prepare the request DTO
                var QuestionAnswer = new OtpQuestionAnswerDTO( questionId,otpAnswer);
                // call the auth API to verify device
                var authResponse = await _userDeviceApi.VerifyDeviceAsync(QuestionAnswer);
                // process the response
                if (authResponse is null)
                    return false;
                else if ((String.IsNullOrEmpty(authResponse.AccessToken) && String.IsNullOrEmpty(authResponse.RefreshToken)) && (authResponse.OtoQuestionId != Guid.Empty))
                {
                    // goto OTO question flow
                    return false;
                }
                else if (!String.IsNullOrEmpty(authResponse.AccessToken) && !String.IsNullOrEmpty(authResponse.RefreshToken)) // all good
                {
                    //  store the tokens
                    var token = new AuthTokens(authResponse.AccessToken, authResponse.RefreshToken, authResponse.ExpiresAtUtc);
                    await _tokenStore.SaveAsync(token);
                    await _authStateProvider.MarkAuthenticatedAsync(token);
                    return true;
                }
                else // something is wrong
                    return false;
            }
            catch
            { return null; }
        }

        public async Task<string?> CreateDeviceIdentifierAsync()
        {
            // check if the device identifier is already set
            if(await _deviceIdStore.GetAsync() != null)
                return null;

            // create a new device identifier, and validate the new id
            var id = ClientSideIdentifier.GetClientSideId();
            if (string.IsNullOrEmpty(id))
                return null;

            // save the new device identifier in the local store
            await _deviceIdStore.SaveAsync(id);
            return id;
        }

        public async Task<bool> UpdateDeviceIdentifierAsync()
        {
            // create a new device identifier
            var id = ClientSideIdentifier.GetClientSideId();

            // validate the new id
            if (string.IsNullOrEmpty(id) || (id == (await _deviceIdStore.GetAsync())) )
                return false;

            // update the device identifier in the Server
            var result = await _userDeviceApi.UpdateDeviceIdAsync(id);
            if (!result)
                return false;

            // save the new device identifier in the local store
            await _deviceIdStore.SaveAsync(id);
            return true;
        }

    }
}
