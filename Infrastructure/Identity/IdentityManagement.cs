using Application.Abstractions.Identity;
using Application.Abstractions.Identity.DTOs;
using Application.Entities.SqlEntities.UsersEntities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Identity
{
    public class IdentityManagement : IIdentityManagement
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppIdentityUser> _userManager;
        private readonly SignInManager<AppIdentityUser> _signInManagement;
        public IdentityManagement(UserManager<AppIdentityUser> userManager, SignInManager<AppIdentityUser> signInManagement,AppDbContext context)
        {
            _userManager = userManager;
            _signInManagement = signInManagement;
            _context = context;
        }

        
        public async Task<bool> CheckPasswordSignInAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return false;
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                    return false;
                var result = await _signInManagement.CheckPasswordSignInAsync(user, password, false);
                return result.Succeeded;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> CheckEmailConfirmedAsync(string? user)
        {
            if (string.IsNullOrWhiteSpace(user))
                return false;
            try
            {
                AppIdentityUser? findUser;
                findUser = await _userManager.FindByEmailAsync(user) ?? await _userManager.FindByNameAsync(user);
                if (findUser == null) return false;
                if (findUser.EmailConfirmed == false) return false;
                return true;
            }
            catch(Exception) { return false; }
        }
        public async Task<bool> CheckUserExistAsync(string value, string type)
        {  if (string.IsNullOrWhiteSpace(value))
                return false;
            return type switch
            {
                "username" => await _userManager.Users.AnyAsync(u => u.UserName == value),
                "email" => await _userManager.Users.AnyAsync(u => u.Email == value),
                _ => false
            };
        }

        public async Task<bool> ConfirmEmailAsync(string email, string token)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
                return false;
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                    return false;
                var result = await _userManager.ConfirmEmailAsync(user, token);
                return result.Succeeded;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<UserIdentityDTO> CreateAsync(string username, string email, string password, string? displayName)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return null!;
            try
            {
                var user = new AppIdentityUser
                {
                    Id = Guid.NewGuid(),
                    UserName = username,
                    Email = email,
                    DisplayName = displayName,
                    CreatedAt = DateTime.UtcNow,
                };
                var result = await _userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    var createdUser = new UserIdentityDTO
                    {
                        Id = user.Id,
                        UserName = username,
                        Email = email,
                        DisplayName = displayName,
                    };
                    return createdUser;
                }
                else
                {
                    var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                    return new UserIdentityDTO { Errors = errors} ;
                }
            }
            catch (Exception)
            {
                return null!;
            }
        }

        public async Task<bool> DeleteAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;
            try
            {
                var findUser = await _userManager.FindByEmailAsync(email);
                if (findUser != null)
                {
                    var resault = await _userManager.DeleteAsync(findUser);
                    if (resault.Succeeded)
                        return true;
                    return false;
                }
                return false;
            }
            catch (Exception) { return false; }
        }

        public async Task<UserIdentityDTO?> FindByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                    return null;
                return new UserIdentityDTO
                {
                    Id = user.Id,
                    UserName = user.UserName!,
                    Email = user.Email!,
                    DisplayName = user.DisplayName
                };
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<UserIdentityDTO?> FindByIdAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                return null;
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                    return null;
                return new UserIdentityDTO
                {
                    Id = user.Id,
                    UserName = user.UserName!,
                    Email = user.Email!,
                    DisplayName = user.DisplayName
                };
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<UserIdentityDTO?> FindByNameAsync(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return null;
            try
            {
                var user = await _userManager.FindByNameAsync(userName);
                if (user == null)
                    return null;
                return new UserIdentityDTO
                {
                    Id = user.Id,
                    UserName = user.UserName!,
                    Email = user.Email!,
                    DisplayName = user.DisplayName
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(string email)
        {
            if (String.IsNullOrEmpty(email))
                return null!;
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                    return null!;
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                return token;
            }
            catch (Exception)
            {
                return null!;
            }
        }
        public async Task<string> GeneratePasswordResetTokenAsync(string email)
        {
            if (String.IsNullOrEmpty(email))
                return null!;
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                    return null!;
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                return token;
            }
            catch (Exception)
            {
                return null!;
            }
        }

        public async Task<string?> ResetPasswordAsync(string email, string resetPwToken, string password)
        {
            if(String.IsNullOrEmpty(email) || String.IsNullOrEmpty(resetPwToken) || String.IsNullOrEmpty(password))
                return null!;
            try
            {
                var findUser = await _userManager.FindByEmailAsync(email);
                if(findUser == null)
                    return null!;
                var resetResault = await _userManager.ResetPasswordAsync(findUser,resetPwToken,password);
                if (resetResault.Succeeded)
                    return "Done";
                var errors = string.Join("; ", resetResault.Errors.Select(e => e.Description));
                return errors;
            }
            catch { return null!; }
        }

        public async Task<Guid> GetAppUserIdAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                return Guid.Empty;
            try
            {
                var appUserId = await _context.AppUsers.Where(AU => AU.IdentityUserId == userId).Select(au => au.Id).SingleOrDefaultAsync();
                if (appUserId == Guid.Empty)
                    return Guid.Empty;
                return appUserId;
            }
            catch (Exception)
            {
                return Guid.Empty;
            }
        }
    }
}
