using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System.Security.Cryptography;

namespace Mini_Social_Media.AppService {
    public class UploadService : IUploadService {
        private readonly Cloudinary _cloudinary;

        public UploadService(Cloudinary cloudinary) {
            _cloudinary = cloudinary;
        }

        public async Task<string?> UploadAsync(IFormFile file) {
            if (file == null || file.Length == 0)
                return null;

            string fileHash = ComputeFileHash(file);

            var uploadParams = new ImageUploadParams() {
                File = new FileDescription(file.FileName, file.OpenReadStream()),
                PublicId = fileHash,
                Overwrite = false
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.Error != null) {
                if (uploadResult.Error.Message.Contains("already exists")) {
                    Console.WriteLine("⚠️ File đã tồn tại trên Cloudinary");
                    return _cloudinary.Api.UrlImgUp.BuildUrl($"{fileHash}.jpg");
                }

                Console.WriteLine("❌ Lỗi upload: " + uploadResult.Error.Message);
                return null;
            }

            return uploadResult.SecureUrl.ToString();
        }

        public string ComputeFileHash(IFormFile file) {
            using var sha = SHA256.Create();
            using var stream = file.OpenReadStream();
            var hashBytes = sha.ComputeHash(stream);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }

    }

}
