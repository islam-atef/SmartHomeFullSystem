using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Image
{
    public interface IImageService
    {
        Task<string?> UploadImagesAsync(IFormFile image, string src);
        void DeleteImage(string src);   
    }
}
