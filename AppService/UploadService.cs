using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System.Security.Cryptography;

namespace Mini_Social_Media.AppService {
    public class UploadService : IUploadService {
        private readonly Cloudinary _cloudinary;

        public UploadService(Cloudinary cloudinary) {
            _cloudinary = cloudinary;
        }

        // ===================
        // Tính SHA-256 hash
        // ===================
        public async Task<string> ComputeFileHashAsync(IFormFile file) {
            using var sha256 = SHA256.Create();
            using var stream = file.OpenReadStream();

            byte[] hashBytes = await sha256.ComputeHashAsync(stream);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }

        // ===================
        // Upload 1 file
        // ===================
        public async Task<string?> UploadAsync(IFormFile file) {
            if (file == null || file.Length == 0)
                return null;

            // 1. Tính Hash để làm PublicId (tránh trùng lặp)
            string fileHash = await ComputeFileHashAsync(file);

            // 2. Mở stream để upload
            using var stream = file.OpenReadStream();

            // ===================
            // VIDEO (SỬA Ở ĐÂY)
            // ===================
            if (file.ContentType.StartsWith("video")) {
                var uploadParams = new VideoUploadParams {
                    File = new FileDescription(file.FileName, stream),
                    PublicId = fileHash,
                    Overwrite = false,
                    // Optional: Eager transformations hoặc notification URL nếu cần
                };

                // QUAN TRỌNG: Dùng UploadLargeAsync thay vì UploadAsync
                // Hàm này tự động chia nhỏ file (Chunked Upload)
                VideoUploadResult result = await _cloudinary.UploadLargeAsync(uploadParams);

                if (result.Error != null) {
                    Console.WriteLine("❌ Cloudinary Video Error: " + result.Error.Message);
                    // Nếu lỗi là do file đã tồn tại (vì ta dùng hash làm ID và Overwrite=false)
                    // thì vẫn trả về URL của file cũ
                    if (result.Error.Message.Contains("already exists")) {
                        return result.SecureUrl?.ToString() ?? result.Url?.ToString();
                    }
                    return null;
                }

                return result.SecureUrl?.ToString();
            }

            // ===================
            // IMAGE
            // ===================
            var imageParams = new ImageUploadParams {
                File = new FileDescription(file.FileName, stream),
                PublicId = fileHash,
                Overwrite = false,
            };

            ImageUploadResult imgResult = await _cloudinary.UploadAsync(imageParams);

            if (imgResult.Error != null) {
                Console.WriteLine("❌ Cloudinary Image Error: " + imgResult.Error.Message);

                // Xử lý trường hợp ảnh đã tồn tại
                if (imgResult.Error.Message.Contains("already exists")) {
                    // Lưu ý: Cloudinary API có thể trả về cấu trúc khác nhau khi lỗi,
                    // nhưng logic cơ bản là nếu trùng thì ta lấy link cũ.
                    // Cách tốt hơn là check resource trước, nhưng để đơn giản ta return null hoặc handle ở UI.
                    // Ở đây mình return null để báo lỗi, hoặc bạn có thể construct URL thủ công dựa trên PublicId.
                }
                return null;
            }

            return imgResult.SecureUrl?.ToString();
        }
    }
}