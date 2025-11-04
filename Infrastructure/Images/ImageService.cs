using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Application.Abstractions.Image;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Images
{
    internal class ImageService(IFileProvider fileProvider) : IImageService
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

        public async Task<List<string>> UploadImagesAsync(IFormFileCollection files, string src)
        {
            try
            {
                // Initialize a list to hold the saved image paths
                var SaveImagesSrc = new List<string>();
                // Define the directory path where images will be saved
                var ImagesDirectory = Path.Combine("wwwroot", "Images", src);
                // Ensure the directory exists
                if (!Directory.Exists(ImagesDirectory))
                    // Create the directory if it doesn't exist
                    Directory.CreateDirectory(ImagesDirectory);
                // Iterate through each file in the collection
                foreach (var image in files)
                {
                    if (image.Length > 0)
                    {
                        // Generate a unique ImageName using a GUID
                        /*
                         *	Guid.NewGuid()
                         *   •	Generates a new, unique identifier (a GUID).
                         *   •	This helps ensure that each image file gets a unique name, avoiding filename collisions.
                         */
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
                        // Add the image source path to the list
                        SaveImagesSrc.Add(ImageSrc);
                    }
                }
                // Return the list of saved image paths
                return SaveImagesSrc;
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during the upload process
                throw new Exception("An error occurred while uploading images.", ex);
            }
            finally
            {
                // Optionally, you can perform any cleanup or logging here
            }
        }
    }
}
