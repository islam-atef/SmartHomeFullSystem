using System.Net.Http.Json;
using Web.Core.UserDevice;
using Web.Models.AuthModel;
using Web.Models.UserDeviceModel;
using static System.Net.WebRequestMethods;

namespace Web.Infrastructure.Http
{
    public class UserDeviceApi : IUserDeviceApi
    {
        private readonly HttpClient _http;
        public UserDeviceApi(HttpClient http) => _http = http;
        private const string BaseUrl = "https://localhost:7072/api/DevicesAuth";

        public async Task<AuthResponseDTO?> VerifyDeviceAsync(OtpQuestionAnswerDTO otpAnswer)
        {
            var response = await _http.PostAsJsonAsync($"{BaseUrl}/VerifyDevice", new
            {
                otoQuestionId = otpAnswer.OtpQuestionId,
                otpAnswer = otpAnswer.OtpAnswer
            });
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<AuthResponseDTO>();
        }

        public async Task<bool> UpdateDeviceIdAsync(string newDeviceId)
        {
            var response = await _http.PostAsJsonAsync($"{BaseUrl}/UpdateDeviceIdentifier", newDeviceId);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<bool>();
        }
    }
}
