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
        public async Task<MyProfileViewModel?> GetMyProfileAsync(int userId) {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return null;
            return new MyProfileViewModel {
                UserName = user.UserName,
                FullName = user.FullName,
                Bio = user.Bio,
                AvatarUrl = user.AvatarUrl,
                WebsiteUrl = user.WebsiteUrl,
                IsPrivate = user.IsPrivate,
                Gender = user.Gender,
                FollowerCount = user.FollowerCount,
                FollowingCount = user.FollowingCount,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                Posts = user.Posts.Select(p => new PostSummaryViewModel {
                    PostId = p.PostId,
                    LikeCount = p.LikeCount,
                    CommentCount = p.CommentCount,
                    UserId = p.UserId,
                    MediaUrl = p.Medias != null && p.Medias.Count > 0 ? p.Medias[0].Url : null
                }).ToList()
            };
        }
        public async Task<UserProfileViewModel?> GetUserProfileAsync(int userId, int requesterId) {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return null;
            bool isFollowing = user.Followers.Any(f => f.FollowerId == requesterId);
            return new UserProfileViewModel {
                UserId = user.Id,
                UserName = user.UserName,
                FullName = user.FullName,
                Bio = user.Bio,
                AvatarUrl = user.AvatarUrl,
                WebsiteUrl = user.WebsiteUrl,
                IsPrivate = user.IsPrivate,
                CreatedAt = user.CreatedAt,
                FollowerCount = user.Followers.Count,
                FollowingCount = user.Following.Count,
                IsFollowing = isFollowing,
                Gender = user.Gender,
                Posts = user.Posts.Select(p => new PostSummaryViewModel {
                    PostId = p.PostId,
                    LikeCount = p.LikeCount,
                    CommentCount = p.CommentCount,
                    UserId = p.UserId,
                    MediaUrl = p.Medias != null && p.Medias.Count > 0 ? p.Medias.First().Url : null
                }).ToList(),
            };
        }
        public async Task<MyProfileViewModel> UpdateUserAvatar(IFormFile file, int userId) {
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
            return new MyProfileViewModel {
                UserName = user.UserName,
                FullName = user.FullName,
                Bio = user.Bio,
                AvatarUrl = user.AvatarUrl,
                WebsiteUrl = user.WebsiteUrl,
                IsPrivate = user.IsPrivate,
                Gender = user.Gender,
                FollowerCount = user.FollowerCount,
                FollowingCount = user.FollowingCount,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                Posts = user.Posts.Select(p => new PostSummaryViewModel {
                    PostId = p.PostId,
                    LikeCount = p.LikeCount,
                    CommentCount = p.CommentCount,
                    UserId = p.UserId,
                    MediaUrl = p.Medias != null && p.Medias.Count > 0 ? p.Medias[0].Url : null
                }).ToList()
            };
        }
        public async Task<EditProfileViewModel> GetEditProfile(int userId) {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return null;
            return new EditProfileViewModel() {
                FullName = user?.FullName,
                Bio = user?.Bio,
                AvatarUrl = user?.AvatarUrl,
                WebsiteUrl = user?.WebsiteUrl,
                Gender = user?.Gender
            };
        }
        public async Task<EditProfileViewModel> Edit(EditProfileInputModel editProfileInputModel, int userId) {
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
            return new EditProfileViewModel() {
                FullName = updateduser?.FullName,
                Bio = updateduser?.Bio,
                AvatarUrl = updateduser?.AvatarUrl,
                WebsiteUrl = updateduser?.WebsiteUrl,
                Gender = updateduser?.Gender,
                UserName = updateduser?.UserName,
                UpdatedAt = updateduser.UpdatedAt,
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
        public async Task<IEnumerable<UserSummaryViewModel>> SearchUsers(string searchinfo) {
            var users = await _userRepository.SearchUsersAsync(searchinfo);
            if (users == null)
                return new List<UserSummaryViewModel>();
            return users.Select(u => new UserSummaryViewModel {
                UserId = u.Id,
                FullName = u.FullName,
                AvatarUrl = u.AvatarUrl,
                UserName = u.UserName
            }).ToList();
        }
    }
}
