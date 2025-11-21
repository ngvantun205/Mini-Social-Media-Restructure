using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace Mini_Social_Media.AppService {
    public class UploadService : IUploadService {
        private readonly Cloudinary _cloudinary;

        public UploadService(Cloudinary cloudinary) {
            _cloudinary = cloudinary;
        }

        public async Task<string> UploadAsync(IFormFile file) {
            if (file.Length <= 0)
                return null;

            await using var stream = file.OpenReadStream();

            var upload = new ImageUploadParams {
                File = new FileDescription(file.FileName, stream),
                Folder = "mini_social_media"
            };

            var result = await _cloudinary.UploadAsync(upload);

            return result?.SecureUrl?.ToString();
        }
    }

}
