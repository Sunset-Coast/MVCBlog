﻿using TechnicalBlog.Services.Interfaces;

namespace TechnicalBlog.Services
{
    public class ImageService : IImageService
    {

        private readonly string _defaultBlogPostImageSrc = "/img/coffeePlant.jpg";
        private readonly string _defaultCategoryImageSrc = "/img/Ball.png";
        private readonly string _defaultUserImageSrc = "/img/GirlOne@4x.png";


        public string ConvertByteArrayToFile(byte[] fileData, string extension, int defaultImage)
        {
            if (fileData == null || fileData.Length == 0)
            {
                switch (defaultImage)
                {
                    case 1: return _defaultUserImageSrc;
                    case 2: return _defaultBlogPostImageSrc;
                    case 3: return _defaultCategoryImageSrc;
                }

                
            }

            try
            {
                string imageBase64Data = Convert.ToBase64String(fileData!);
                string imageSrcString = string.Format($"data:{extension};base64,{imageBase64Data}");

                return imageSrcString;

            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<byte[]> ConvertFileToByteArrayAsync(IFormFile file)
        {
            try
            {
                using MemoryStream memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);
                byte[] byteFile = memoryStream.ToArray();
                memoryStream.Close();
                return byteFile;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
