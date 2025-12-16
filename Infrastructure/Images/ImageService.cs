using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Application.Abstractions.Image;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Images
{
    internal class ImageService(IFileProvider fileProvider,ILogger<ImageService> logger) : IImageService
    {
        public void DeleteImage(string src)
        {
            try
            {
                // Get the file information for the specified image source
                var info = fileProvider.GetFileInfo(src);
                // Check if the file exists
                if (info.Exists)
                {
                    // If the file exists, delete it
                    File.Delete(info.PhysicalPath!);
                }
                else
                {
                    // If the file does not exist, you can throw an exception or handle it as needed
                    throw new FileNotFoundException($"The image at {src} does not exist.");
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during the deletion process
                throw new Exception("An error occurred while deleting the image.", ex);
            }
            finally
            {
                // Optionally, you can perform any cleanup or logging here
            }
        }

        public async Task<string?> UploadImagesAsync(IFormFile image, string src)
        {
            try
            {
                // Initialize a list to hold the saved image paths
                string SaveImagesSrc = null!;
                // Define the directory path where images will be saved
                var ImagesDirectory = Path.Combine("wwwroot", "Images", src);
                // Ensure the directory exists
                if (!Directory.Exists(ImagesDirectory))
                    // Create the directory if it doesn't exist
                    Directory.CreateDirectory(ImagesDirectory);
                if (image.Length > 0)
                {
                    // Generate a unique ImageName using a GUID
                    var ImageName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                    // save the source path for the image
                    var ImageSrc = $"/Images/{src}/{ImageName}";
                    // Combine the directory path and the image name to get the full path   
                    var ImagePath = Path.Combine(ImagesDirectory, ImageName);
                    // Open a file stream to write the image to the specified path
                    using (var stream = new FileStream(ImagePath, FileMode.Create))
                    {
                        // Copy the image file to the stream
                        await image.CopyToAsync(stream);
                    }
                    // save the image source path
                    SaveImagesSrc = ImageSrc;
                }
                // Return the list of saved image paths
                return SaveImagesSrc;
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during the upload process
                logger.LogCritical("ImageService :UploadImagesAsync: {error}", ex.Message);
                return "error";
            }
            finally
            {
                // Optionally, you can perform any cleanup or logging here
                logger.LogTrace("ImageService :UploadImagesAsync: image has been uploaded successfully");
            }
        }
    }
}
