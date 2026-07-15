using Blog.Application.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Infrastructure.Service
{
    public class CloudinaryFileService : IFileService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryFileService(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }

        public async Task<string> SaveFileAsync(Stream fileStream, string fileName, string[] allowedExtensions)
        {
            var extension = Path.GetExtension(fileName).ToLower();
            if (!allowedExtensions.Contains(extension))
            {
                throw new ArgumentException("Định dạng file không được hỗ trợ.");
            }

            var uploadResult = new ImageUploadResult();

            if (fileStream.Length > 0)
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(fileName, fileStream),
                    Folder = "myblog_uploads",
                    Transformation = new Transformation().Width(1200).Crop("limit").Quality("auto")
                };

                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            }

            if (uploadResult.Error != null)
            {
                throw new Exception($"Lỗi tải ảnh lên Cloudinary: {uploadResult.Error.Message}");
            }

            return uploadResult.SecureUrl.ToString();
        }

        public async Task DeleteFileAsync(string fileUrl)
        {
            if (string.IsNullOrEmpty(fileUrl)) return;

            var publicId = GetPublicIdFromUrl(fileUrl);
            if (string.IsNullOrEmpty(publicId)) return;

            var deletionParams = new DeletionParams(publicId);
            await _cloudinary.DestroyAsync(deletionParams);
        }

        private string? GetPublicIdFromUrl(string url)
        {
            try
            {
                var folderName = "myblog_uploads/";
                var folderIndex = url.IndexOf(folderName);
                if (folderIndex == -1) return null;

                var afterFolder = url.Substring(folderIndex); 
                
                var dotIndex = afterFolder.LastIndexOf('.');
                if (dotIndex != -1)
                {
                    return afterFolder.Substring(0, dotIndex);
                }
                return afterFolder;
            }
            catch
            {
                return null;
            }
        }
    }
}
