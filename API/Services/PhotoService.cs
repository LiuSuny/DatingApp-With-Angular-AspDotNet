using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Helpers;
using API.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace API.Services
{
    public class PhotoService : IPhotoService
    {
         private readonly Cloudinary _cloudinary;
       
        public PhotoService(IOptions<CloudinarySettings> config)
       {
            var myAccount = new Account
            (
            config.Value.CloudName,
            config.Value.ApiKey,
            config.Value.ApiSecret
            );
            _cloudinary = new Cloudinary(myAccount);
            //_cloudinary.Api.Secure = true;

        }

        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
        {
           

            var uploadResult = new ImageUploadResult();
           //checking if file is greater than 0 meaning there file or images inside our file
           if(file.Length > 0)
           {
                using var stream = file.OpenReadStream(); //getting our file as stream of data
                var uploadParam = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Height(500)
                     .Width(500).Crop("fill").Gravity("face"),
                    Folder="da-net8"
                };
                //uploadResult = await _cloudinary.UploadAsync(uploadParam);
                uploadResult = await _cloudinary.UploadAsync(uploadParam);
            }
           return uploadResult;
        }

        public async Task<DeletionResult> DeletePhotoAsync(string publicId)
        {
            
            var deleteParam = new DeletionParams(publicId);

            return await _cloudinary.DestroyAsync(deleteParam);
           
           
        }
    }
}