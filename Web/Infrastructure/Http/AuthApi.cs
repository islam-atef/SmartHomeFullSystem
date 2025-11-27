using System.Net.Http.Json;
using Web.Core.Auth;
using Web.Core.UserDevice;
using Web.Core.Utile;
using Web.Models.AuthModel;

namespace Web.Infrastructure.Http
{
    public class AuthApi : IAuthApi
    {
        private readonly HttpClient _http;
        private readonly IDeviceIdentityStore _deviceIdStore;
        public AuthApi(HttpClient http ,IDeviceIdentityStore deviceIdStore)
        {
            _http = http;
            _deviceIdStore = deviceIdStore;
        }

        private const string BaseUrl = "https://localhost:7072/api/Auth";


        // ========================
        // Helper: Request + Device
        // ========================
        private async Task<HttpRequestMessage> CreateRequestWithDeviceHeaderAsync(HttpMethod method, string url, object? body = null)
        {
            // create the HTTP request message
            var request = new HttpRequestMessage(method, url);
            // add the body if exists
            if (body is not null)
                request.Content = JsonContent.Create(body);
            // get the device id from the device identity store
            var deviceId = await _deviceIdStore.GetAsync();
            if (string.IsNullOrWhiteSpace(deviceId)) // if device id is null, Create a new device id, store it, and use it
            { 
                // create a new device identifier, and validate the new id
                var id = ClientSideIdentifier.GetClientSideId();
                // save the new device identifier in the local store
                await _deviceIdStore.SaveAsync(id);
            }
            // add the device id to the request headers
            request.Headers.Add("Device-MAC", deviceId);
            // return the request message
            return request;
        }
        // ========================

        // ========================
        // API Methods :-----------
        // ========================

        // 1) Refresh the authentication tokens using the provided refresh token
        public async Task<AuthResponseDTO?> RefreshAsync(string refreshToken)
        {
            // create the request body
            var body = new { RefreshToken = refreshToken };
            // create the HTTP request with the device header
            var request = await CreateRequestWithDeviceHeaderAsync(
                HttpMethod.Post,
                $"{BaseUrl}/refresh-token",
                body);
            // send the request
            var res = await _http.SendAsync(request);
            // check if the response is successful
            if (!res.IsSuccessStatusCode)
                return null;
            // read the response content as AuthResponseDTO
            var dto = await res.Content.ReadFromJsonAsync<AuthResponseDTO>();
            // map and return the AuthResponseDTO
            return dto is null
                ? null
                : new AuthResponseDTO(dto.AccessToken, dto.RefreshToken, dto.ExpiresAtUtc, dto.OtoQuestionId);
        }

        public async Task<AuthResponseDTO> LoginAsync(LoginRequestDTO requestModel)
        {
            // create the request body
            var body = new
            {
                email = requestModel.Email,
                username = requestModel.Username,
                password = requestModel.Password
            };
            // create the HTTP request with the device header
            var request = await CreateRequestWithDeviceHeaderAsync(
                HttpMethod.Post,
                $"{BaseUrl}/login",
                body);
            // send the request
            var response = await _http.SendAsync(request);
            // ensure the response is successful, otherwise throw an exception
            response.EnsureSuccessStatusCode();
            // read and return the response content as AuthResponseDTO if the response is successful
            return await response.Content.ReadFromJsonAsync<AuthResponseDTO>() 
                ?? new AuthResponseDTO(null!,null!,DateTime.UtcNow,null);
        }
    }
}
