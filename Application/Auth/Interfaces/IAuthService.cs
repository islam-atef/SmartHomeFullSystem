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
        Task<GenericResult<string>> RegisterAsync(RegisterRequestDTO req);
        Task<GenericResult<bool>> AccountActivation(AccountActivationDTO accountActivationDTO);
        Task<GenericResult<string>> SendEmailForForgottenPassword(string email);
        Task<GenericResult<string>> ResetPassword(ResetPasswordDTO rpw);
        Task<GenericResult<AuthResponseDTO>> LoginAsync(LoginRequestDTO req);
        Task<GenericResult<AuthResponseDTO>> RefreshAsync(string refreshToken);
        Task<GenericResult<bool>> DeleteAccount(string email);
    }
}
