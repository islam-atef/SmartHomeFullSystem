using Application.Abstractions.Image;
using Application.Entities.SqlEntities.UsersEntities;
using Application.GenericResult;
using Application.RepositotyInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class AppUserRepo : IAppUserRepo
    {
        private readonly AppDbContext _context;

        public AppUserRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<GenericResult<AppUser>> AddUserAsync(AppUser user, CancellationToken ct = default)    
        {
            if(user == null) 
                return GenericResult<AppUser>.Failure(ErrorType.InvalidData, "User cannot be null");
            try
            {
                if(await IsUserExistsAsync(user.Id))
                {
                    return GenericResult<AppUser>.Failure(ErrorType.Conflict, "A user with the same Id already exists.");
                }
                await _context.AppUsers.AddAsync(user,ct);

                var _savingResult = await _context.SaveChangesAsync(ct);
                if (_savingResult <= 0)
                    return GenericResult<AppUser>.Failure(ErrorType.DatabaseError, "Failed to add the user to the database.");

                return GenericResult<AppUser>.Success(user,"New User has been added successfully!");
            }
            catch (Exception ex)
            {
                return GenericResult<AppUser>.Failure(ErrorType.DatabaseError, $"An error occurred while adding the user: {ex.Message}");
            }
        }

        public async Task<bool> IsUserExistsAsync(Guid id, CancellationToken ct = default)
            => await _context.AppUsers.AnyAsync(u => u.Id == id, ct);

        public async Task<GenericResult<bool>> RemoveUserAsync(Guid userId, CancellationToken ct = default)
        {
            if (!await IsUserExistsAsync(userId, ct))
                return GenericResult<bool>.Failure(ErrorType.NotFound, "User not found.");
            try
            {
                var user = await _context.AppUsers.FirstOrDefaultAsync(u => u.Id == userId, ct);
                _context.AppUsers.Remove(user!);
                var _savingResult = await _context.SaveChangesAsync(ct);
                if (_savingResult <= 0)
                    return GenericResult<bool>.Failure(ErrorType.DatabaseError, "Failed to remove the user from database.");
                return GenericResult<bool>.Success(true, "User has been removed successfully!");
            }
            catch (Exception ex)
            {
                return GenericResult<bool>.Failure(ErrorType.DatabaseError, $"An error occurred while removing the user: {ex.Message}");
            }
        }

        public async Task<GenericResult<(string email, string userName)>> GetUserIdentityInfoAsync(Guid userId, CancellationToken ct = default)
        {
            if (userId == Guid.Empty)
                return  GenericResult<(string email, string userName)>.Failure(ErrorType.InvalidData, "UserId cannot be empty");
            try
            {
                var user = await _context.AppUsers
                    .Include(u => u.IdentityUser)
                    .FirstOrDefaultAsync(u => u.Id == userId, ct);
                if (user == null || user.IdentityUser == null || String.IsNullOrEmpty(user.IdentityUser.UserName) || String.IsNullOrEmpty(user.IdentityUser.Email))
                    return GenericResult<(string email, string userName)>.Failure(ErrorType.NotFound, "User not found.");

                (string _email, string _userName) = (user.IdentityUser.Email , user.IdentityUser.UserName);

                return GenericResult<(string email, string userName)>.Success(
                    (_email, _userName),
                    "User info retrieved successfully.");
            }
            catch (Exception ex)
            {
                return GenericResult<(string email, string userName)>.Failure(ErrorType.DatabaseError, $"An error occurred while retrieving user info: {ex.Message}");
            }
        }
        public async Task<GenericResult<(string email, string userName, string displayName, string phone)>> GetUserInfoAsync(Guid userId, CancellationToken ct = default)
        {
            if (userId == Guid.Empty)
                return GenericResult<(string email, string userName, string displayName, string phone)>.Failure(ErrorType.InvalidData, "UserId cannot be empty");
            try
            {
                var user = await _context.AppUsers
                    .Include(u => u.IdentityUser)
                    .FirstOrDefaultAsync(u => u.Id == userId, ct);
                if (user == null || user.IdentityUser == null || String.IsNullOrEmpty(user.IdentityUser.UserName) || String.IsNullOrEmpty(user.IdentityUser.Email))
                    return GenericResult<(string email, string userName, string displayName, string phone)>.Failure(ErrorType.NotFound, "User not found.");

                (string _email, string _userName, string displayName, string phone) =
                    (user.IdentityUser.Email, user.IdentityUser.UserName, user.IdentityUser.DisplayName ?? "Not Exist", user.IdentityUser.PhoneNumber ?? "Not Exist");

                return GenericResult<(string email, string userName, string displayName, string phone)>.Success(
                    (_email, _userName, displayName, phone),
                    "User info retrieved successfully.");
            }
            catch (Exception ex)
            {
                return GenericResult<(string email, string userName, string displayName, string phone)>.Failure(ErrorType.DatabaseError, $"An error occurred while retrieving user info: {ex.Message}");
            }
        }

        public async Task<GenericResult<bool>> ChangeUserDisplayNameAsync(Guid userId, string newName, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(newName) || userId == Guid.Empty)
            {
                return GenericResult<bool>.Failure(ErrorType.MissingData, "there are missing data!"); 
            }
            try
            {
                // 1- get the user.
                var user = await _context.AppUsers
                    .Include(u => u.IdentityUser)
                    .FirstOrDefaultAsync(u => u.Id == userId, ct);
                if (user == null)    
                { 
                    return GenericResult<bool>.Failure(ErrorType.NotFound, "there is no user with this ID!");
                }

                // 2- modifiy user DisplayName
                user!.IdentityUser.DisplayName = newName.Trim(); 

                // 3- save changes
                var result = await _context.SaveChangesAsync(ct);
                if (result <= 0)
                { 
                    return GenericResult<bool>.Failure(ErrorType.Conflict, "there is an error while saving changes!"); 
                }
                return GenericResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return GenericResult<bool>.Failure(ErrorType.Conflict, ex.Message);
            }
        }
        public async Task<GenericResult<bool>> ChangeUserPhoneNumberAsync(Guid userId, string newNumber, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(newNumber) || userId == Guid.Empty)
            {
                return GenericResult<bool>.Failure(ErrorType.MissingData, "there are missing data!");
            }
            try
            {
                // 1- get the user.
                var user = await _context.AppUsers
                    .Include(u => u.IdentityUser)
                    .FirstOrDefaultAsync(u => u.Id == userId, ct);
                if (user == null)
                {
                    return GenericResult<bool>.Failure(ErrorType.NotFound, "there is no user with this ID!");
                }

                // 2- modifiy user PhoneNumber
                user!.IdentityUser.PhoneNumber = newNumber.Trim();

                // 3- save changes
                var result = await _context.SaveChangesAsync(ct);
                if (result <= 0)
                {
                    return GenericResult<bool>.Failure(ErrorType.Conflict, "there is an error while saving changes!");
                }
                return GenericResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return GenericResult<bool>.Failure(ErrorType.Conflict, ex.Message);
            }
        }

        public async Task<GenericResult<bool>> ChangeUserNameAsync(Guid userId, string newUserName, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(newUserName) || userId == Guid.Empty)
            {
                return GenericResult<bool>.Failure(ErrorType.MissingData, "there are missing data!");
            }
            try
            {
                // 1- get the user.
                var user = await _context.AppUsers
                    .Include(u => u.IdentityUser)
                    .FirstOrDefaultAsync(u => u.Id == userId, ct);
                if (user == null)
                {
                    return GenericResult<bool>.Failure(ErrorType.NotFound, "there is no user with this ID!");
                }

                // 2- modifiy user UserName
                user!.IdentityUser.UserName = newUserName.Trim();

                // 3- save changes
                var result = await _context.SaveChangesAsync(ct);
                if (result <= 0)
                {
                    return GenericResult<bool>.Failure(ErrorType.Conflict, "there is an error while saving changes!");
                }
                return GenericResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return GenericResult<bool>.Failure(ErrorType.Conflict, ex.Message);
            }
        }
    }
}
