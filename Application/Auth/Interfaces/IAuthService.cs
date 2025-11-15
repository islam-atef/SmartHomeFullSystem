using Application.Auth.DTOs;
using Application.GenericResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Auth.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDTO?> IssueTokensAsync(Guid userId, string email, Guid deviceId);
        Task<GenericResult<string>> RegisterAsync(RegisterDTO req);
        Task<GenericResult<bool>> AccountActivation(AccountActivationDTO accountActivationDTO);
        Task<GenericResult<string>> SendEmailForForgottenPassword(string email);
        Task<GenericResult<string>> ResetPassword(ResetPasswordDTO rpw);
        Task<GenericResult<AuthResponseDTO>> LoginAsync(LoginDTO req);
        Task<GenericResult<bool>> LogoutAsync(LogoutDTO req);
        Task<GenericResult<AuthResponseDTO>> RefreshAsync(string refreshToken, string deviceMAC);
        Task<GenericResult<bool>> DeleteAccount(string email);
    }
}
