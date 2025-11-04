using Application.Abstractions.Identity.DTOs;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Identity
{
    public interface IIdentityManagement
    {
        Task<string> GenerateEmailConfirmationTokenAsync(string email);
        Task<string> GeneratePasswordResetTokenAsync(string email);

        Task<UserIdentityDTO?> FindByEmailAsync(string email);
        Task<UserIdentityDTO?> FindByIdAsync(Guid userId);
        Task<UserIdentityDTO?> FindByNameAsync(string userName);

        Task<bool> CheckPasswordSignInAsync(string email, string password);
        Task<bool> CheckEmailConfirmedAsync(string? user);

        Task<bool> ConfirmEmailAsync(string email, string token);

        Task<UserIdentityDTO> CreateAsync(string username, string email, string password, string? displayName);
        Task<bool> DeleteAsync(string email);

        Task<string?> ResetPasswordAsync(string email, string resetPwToken, string password);
    }
}
 