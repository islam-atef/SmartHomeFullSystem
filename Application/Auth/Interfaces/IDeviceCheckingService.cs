using Application.Auth.DTOs;
using Application.GenericResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Auth.Interfaces
{
    public interface IDeviceCheckingService
    {
        Task<GenericResult<bool>> SendDeviceCheckingOtpAsync(LoginRequestDTO req, Guid appUserId);
        Task<GenericResult<AuthResponseDTO>> VerifyOtpAsync(Guid appUserId, int deviceCheckingCode);
    }
}
