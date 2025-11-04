using Application.Abstractions.Image;
using Domain.Entities.SqlEntities.UsersEntities;
using Domain.GenericResult;
using Domain.RepositotyInterfaces;
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
        private readonly IImageService _image;

        public AppUserRepo(AppDbContext context, IImageService image)
        {
            _context = context;
            _image = image;
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


        public async Task<GenericResult<string?>> AddOrUpdateUserImageAsync(IFormFileCollection imageFiles,AppUser user,CancellationToken ct = default)
        {
            // 1) Validate input
            if (imageFiles is null || imageFiles.Count == 0)
                return GenericResult<string?>.Failure(ErrorType.InvalidData, "No image provided.");
            try
            {
                // 2) Bring a tracked user (and load IdentityUser if you need UserName)
                var existingUser = await _context.AppUsers
                    .Include(u => u.IdentityUser)
                    .FirstOrDefaultAsync(u => u.Id == user.Id, ct);

                if (existingUser is null)
                    return GenericResult<string?>.Failure(ErrorType.NotFound, "User not found.");

                var oldImageUrl = existingUser.UserImageUrl;

                // 3) Upload the new image FIRST (avoid losing old if upload fails)
                var uploaded = await _image.UploadImagesAsync(imageFiles, existingUser.IdentityUser?.UserName ?? existingUser.Id.ToString());
                if (uploaded is null || uploaded.Count == 0 || string.IsNullOrWhiteSpace(uploaded[0]))
                    return GenericResult<string?>.Failure(ErrorType.InvalidData, "Upload failed or returned empty URL.");

                var newImageUrl = uploaded[0];

                // 4) Update entity
                existingUser.UserImageUrl = newImageUrl;

                // 5) Save DB changes
                var affected = await _context.SaveChangesAsync(ct);
                if (affected <= 0)
                {
                    // Rollback the uploaded file to avoid orphan files
                    try { _image.DeleteImage(newImageUrl); } catch { /* swallow */ }
                    return GenericResult<string?>.Failure(ErrorType.DatabaseError, "Failed to update user in database.");
                }

                // 6) After successful save, delete old file (best-effort)
                if (!string.IsNullOrWhiteSpace(oldImageUrl))
                {
                    try { _image.DeleteImage(oldImageUrl); } catch { /* swallow */ }
                }

                return GenericResult<string?>.Success(newImageUrl, "Image has been added/updated successfully.");
            }
            catch (Exception ex)
            {
                return GenericResult<string?>.Failure(ErrorType.Conflict, ex.Message);
            }
        }


        public async Task<GenericResult<AppUser>> UpdateUserAsync(AppUser user, CancellationToken ct = default)
        {
            if (user == null)
                return GenericResult<AppUser>.Failure(ErrorType.InvalidData, "User cannot be null");
            try
            {
                if (!await IsUserExistsAsync(user.Id))
                {
                    return GenericResult<AppUser>.Failure(ErrorType.Conflict, "No user with the same Id already exists.");
                }

                _context.AppUsers.Update(user);

                var _savingResult = await _context.SaveChangesAsync(ct);
                if (_savingResult <= 0)
                    return GenericResult<AppUser>.Failure(ErrorType.DatabaseError, "Failed to Update the user at database.");

                return GenericResult<AppUser>.Success(user, "User has been Updated successfully!");
            }
            catch (Exception ex)
            {
                return GenericResult<AppUser>.Failure(ErrorType.DatabaseError, $"An error occurred while Updating the user: {ex.Message}");
            }
        }


        public async Task<bool> IsUserExistsAsync(Guid id, CancellationToken ct = default)
            => await _context.AppUsers.AnyAsync(u => u.Id == id, ct);   
    }
}
