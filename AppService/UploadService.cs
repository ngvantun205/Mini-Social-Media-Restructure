using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System.Security.Cryptography;

namespace Mini_Social_Media.AppService {
    public class UploadService : IUploadService {
        private readonly Cloudinary _cloudinary;

        public UploadService(Cloudinary cloudinary) {
            _cloudinary = cloudinary;
        }
        public async Task<string> ComputeFileHashAsync(IFormFile file) {
            using var sha256 = SHA256.Create();
            using var stream = file.OpenReadStream();

            byte[] hashBytes = await sha256.ComputeHashAsync(stream);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
        public async Task<string?> UploadAsync(IFormFile file) {
            if (file == null || file.Length == 0)
                return null;

            string fileHash = await ComputeFileHashAsync(file);

            using var stream = file.OpenReadStream();

            if (file.ContentType.StartsWith("video") || file.ContentType.StartsWith("audio")) {
                var uploadParams = new VideoUploadParams {
                    File = new FileDescription(file.FileName, stream),
                    PublicId = fileHash,
                    Overwrite = false,
                };

                VideoUploadResult result = await _cloudinary.UploadLargeAsync(uploadParams);

                if (result.Error != null) {
                    Console.WriteLine("❌ Cloudinary Video/Audio Error: " + result.Error.Message);
                    if (result.Error.Message.Contains("already exists")) {
                        return result.SecureUrl?.ToString() ?? result.Url?.ToString();
                    }
                    return null;
                }

                return result.SecureUrl?.ToString();
            }

            var imageParams = new ImageUploadParams {
                File = new FileDescription(file.FileName, stream),
                PublicId = fileHash,
                Overwrite = false,
            };

            ImageUploadResult imgResult = await _cloudinary.UploadAsync(imageParams);

            if (imgResult.Error != null) {
                Console.WriteLine("❌ Cloudinary Image Error: " + imgResult.Error.Message);

                if (imgResult.Error.Message.Contains("already exists")) {
                }
                return null;
            }

            return imgResult.SecureUrl?.ToString();
        }
    }
}