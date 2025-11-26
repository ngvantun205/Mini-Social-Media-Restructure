using Mini_Social_Media.Models.DomainModel;
using System.Security.Cryptography.X509Certificates;

namespace Mini_Social_Media.AppService {
    public class UserService : IUserService {
        private readonly IUserRepository _userRepository;
        private readonly IUploadService _uploadService;
        public UserService(IUserRepository userRepository, IUploadService uploadService) {
            _userRepository = userRepository;
            _uploadService = uploadService;
        }
        public async Task<MyProfileDto?> GetMyProfileAsync(int userId) {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return null;
            return new MyProfileDto {
                UserName = user.UserName,
                FullName = user.FullName,
                Bio = user.Bio,
                AvatarUrl = user.AvatarUrl,
                WebsiteUrl = user.WebsiteUrl,
                IsPrivate = user.IsPrivate,
                Gender = user.Gender,
                FollowersCount = user.Followers.Count,
                FollowingCount = user.Following.Count,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                Posts = user.Posts.Select(p => new PostSummaryDto {
                    PostId = p.PostId,
                    LikeCount = p.LikeCount,
                    CommentCount = p.CommentCount,
                    UserId = p.UserId,
                    MediaUrl = p.Medias != null && p.Medias.Count > 0 ? p.Medias[0].Url : null
                }).ToList()
            };
        }
        public async Task<UserProfileDto?> GetUserProfileAsync(int userId, int requesterId) {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return null;
            bool isFollowing = user.Followers.Any(f => f.FollowerId == requesterId);
            return new UserProfileDto {
                UserId = user.Id,
                UserName = user.UserName,
                FullName = user.FullName,
                Bio = user.Bio,
                AvatarUrl = user.AvatarUrl,
                WebsiteUrl = user.WebsiteUrl,
                IsPrivate = user.IsPrivate,
                CreatedAt = user.CreatedAt,
                FollowersCount = user.Followers.Count,
                FollowingCount = user.Following.Count,
                IsFollowing = isFollowing,
                Gender = user.Gender,
                Posts = user.Posts.Select(p => new PostSummaryDto {
                    PostId = p.PostId,
                    LikeCount = p.LikeCount,
                    CommentCount = p.CommentCount,
                    UserId = p.UserId,
                    MediaUrl = p.Medias != null && p.Medias.Count > 0 ? p.Medias.First().Url : null
                }).ToList(),
            };
        }
        public async Task<MyProfileDto> UpdateUserAvatar(IFormFile file, int userId) {
            if (file == null)
                return null;
            var url = await _uploadService.UploadAsync(file);
            if (string.IsNullOrEmpty(url)) {
                return null;
            }
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return null;
            user.AvatarUrl = url;
            await _userRepository.UpdateAsync(user);
            return new MyProfileDto {
                UserName = user.UserName,
                FullName = user.FullName,
                Bio = user.Bio,
                AvatarUrl = user.AvatarUrl,
                WebsiteUrl = user.WebsiteUrl,
                IsPrivate = user.IsPrivate,
                Gender = user.Gender,
                FollowersCount = user.Followers.Count,
                FollowingCount = user.Following.Count,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                Posts = user.Posts.Select(p => new PostSummaryDto {
                    PostId = p.PostId,
                    LikeCount = p.LikeCount,
                    CommentCount = p.CommentCount,
                    UserId = p.UserId,
                    MediaUrl = p.Medias != null && p.Medias.Count > 0 ? p.Medias[0].Url : null
                }).ToList()
            };
        }
    }
}
