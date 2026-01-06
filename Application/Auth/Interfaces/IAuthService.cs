using Application.Auth.DTOs;
using Domain.GenericResult;
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
        Task<GenericResult<bool>> RegisterAsync(RegisterDTO req);
        Task<GenericResult<bool>> AccountActivation(AccountActivationDTO accountActivationDTO);
        Task<GenericResult<bool>> SendEmailForForgottenPassword(string email);
        Task<GenericResult<bool>> ResetPassword(ResetPasswordDTO rpw);
        Task<GenericResult<AuthResponseDTO>> LoginAsync(LoginDTO req);
        Task<GenericResult<bool>> LogoutAsync(LogoutDTO req);
        Task<GenericResult<AuthResponseDTO>> RefreshAsync(string refreshToken, string deviceMAC);
        Task<GenericResult<bool>> CheckEmailExistanceAsync(string email);
        Task<GenericResult<bool>> CheckUserNameExistanceAsync(string username);
        Task<GenericResult<bool>> DeleteAccount(string email);
    }
}
