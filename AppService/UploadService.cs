using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace Mini_Social_Media.AppService {
    public class UploadService : IUploadService {
        private readonly Cloudinary _cloudinary;

        public UploadService(Cloudinary cloudinary) {
            _cloudinary = cloudinary;
        }

        public async Task<string?> UploadAsync(IFormFile file) {
            if (file == null || file.Length == 0) {
                Console.WriteLine("❌ File rỗng hoặc không tồn tại");
                return null;
            }

            using (var stream = file.OpenReadStream()) {
                var uploadParams = new ImageUploadParams() {
                    File = new FileDescription(file.FileName, stream)
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult == null) {
                    Console.WriteLine("❌ uploadResult null → Upload thất bại");
                    return null;
                }

                if (uploadResult.Error != null) {
                    Console.WriteLine("❌ Cloudinary Error: " + uploadResult.Error.Message);
                    return null;
                }

                Console.WriteLine("✅ Upload thành công → URL: " + uploadResult.SecureUrl);

                return uploadResult.SecureUrl?.ToString();
            }
        }

    }

}
