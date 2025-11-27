using Web.Models.AuthModel;
using Web.Models.UserDeviceModel;

namespace Web.Core.UserDevice
{
    public interface IUserDeviceApi 
    {
        Task<AuthResponseDTO?> VerifyDeviceAsync(OtpQuestionAnswerDTO otpAnswer);
        Task<bool> UpdateDeviceIdAsync(string deviceId);
    }
}
