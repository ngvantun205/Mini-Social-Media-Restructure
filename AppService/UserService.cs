using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace Mini_Social_Media.AppService {
    public class UserService : IUserService {
        private readonly IUserRepository _userRepository;
        private readonly IUploadService _uploadService;
        private readonly UserManager<User> _userManager;
        private readonly IHubContext<NotificationsHub> _hubContext;
        public UserService(IUserRepository userRepository, IUploadService uploadService, UserManager<User> userManager, IHubContext<NotificationsHub> hubContext) {
            _userRepository = userRepository;
            _uploadService = uploadService;
            _userManager = userManager;
            _hubContext = hubContext;
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
                FollowersCount = user.FollowerCount,
                FollowingCount = user.FollowingCount,
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
                FollowersCount = user.FollowerCount,
                FollowingCount = user.FollowingCount,
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
        public async Task<EditProfileDto> GetEditProfile(int userId) {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return null;
            return new EditProfileDto() {
                FullName = user?.FullName,
                Bio = user?.Bio,
                AvatarUrl = user?.AvatarUrl,
                WebsiteUrl = user?.WebsiteUrl,
                Gender = user?.Gender
            };
        }
        public async Task<EditProfileDto> Edit(EditProfileInputModel editProfileInputModel, int userId) {
            if (editProfileInputModel == null)
                return null;
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return null;
            user.FullName = editProfileInputModel.FullName;
            user.Bio = editProfileInputModel?.Bio;
            user.WebsiteUrl = editProfileInputModel?.WebsiteUrl;
            user.Gender = editProfileInputModel?.Gender;
            var url = await _uploadService.UploadAsync(editProfileInputModel.AvatarUrl);
            if (url != null)
                user.AvatarUrl = url;
            await _userRepository.UpdateAsync(user);
            var updateduser = await _userRepository.GetByIdAsync(userId);
            return new EditProfileDto() {
                FullName = updateduser?.FullName,
                Bio = updateduser?.Bio,
                AvatarUrl = updateduser?.AvatarUrl,
                WebsiteUrl = updateduser?.WebsiteUrl,
                Gender = updateduser?.Gender,
            };
        }
        public async Task<IdentityResult> ChangePassword(ChangePasswordInputModel changeInput, int userId) {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });

            if (changeInput.NewPassword != changeInput.ComfirmNewPassword)
                return IdentityResult.Failed(new IdentityError { Description = "Passwords do not match" });

            var correctOldPassword = await _userManager.CheckPasswordAsync(user, changeInput.Password);
            if (!correctOldPassword)
                return IdentityResult.Failed(new IdentityError { Description = "Current password is incorrect" });

            var result = await _userManager.ChangePasswordAsync(user, changeInput.Password, changeInput.NewPassword);
            return result;
        }
        public async Task ChangeAccountPrivacy(bool isPrivate, int userId) {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return;
            await _userRepository.UpdatePrivacy(isPrivate, userId);
        }
    }
}
