using Application.Auth.DTOs;
using Application.GenericResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Auth.Interfaces
{
    public interface IDeviceManagementService
    {
        Task<GenericResult<Guid>> SendDeviceCheckingOtpAsync(LoginDTO req, Guid appUserId);
        Task<GenericResult<AuthResponseDTO>> VerifyOtpAsync(Guid questionId, int otpAnswer, string deviceMAC);
        Task<GenericResult<bool>> UpdateDeviceMACAsync(string oldDeviceMAC, string newDeviceMAC);
    }
}
