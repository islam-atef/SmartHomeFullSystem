using System.Net.Http.Json;
using Web.Core.Auth;
using Web.Models.AuthModel;

namespace Web.Infrastructure.Http
{
    public class AuthApi : IAuthApi
    {
        private readonly HttpClient _http;
        public AuthApi(HttpClient http) => _http = http;
        private const string BaseUrl = "/api/auth";

        // Refresh the authentication tokens using the provided refresh token
        public async Task<AuthTokens?> RefreshAsync(string refreshToken)
        {
            // Send POST request to /api/auth/refresh-token with the refresh token
            var res = await _http.PostAsJsonAsync($"{BaseUrl}/refresh-token", new { RefreshToken = refreshToken });

            // If the response is not successful, return null
            if (!res.IsSuccessStatusCode) return null;

            // Read the response content as AuthResponseDTO
            var dto = await res.Content.ReadFromJsonAsync<AuthResponseDTO>();

            // Map AuthResponseDTO to AuthTokens and return
            return dto is null ? null : new AuthTokens(dto.AccessToken, dto.RefreshToken, dto.ExpiresAtUtc);
        }
    }
}
