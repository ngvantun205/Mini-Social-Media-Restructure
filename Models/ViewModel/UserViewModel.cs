namespace Mini_Social_Media.Models.ViewModel {
    public class UserProfileViewModel {
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? FullName { get; set; }
        public string? Bio { get; set; }
        public string? AvatarUrl { get; set; }
        public string? WebsiteUrl { get; set; }
        public bool IsPrivate { get; set; }
        public int PostCount { get; set; }
        public int FollowersCount { get; set; }
        public int FollowingCount { get; set; }
        public bool IsFollowing { get; set; }
        public string? Gender { get; set; }
        public DateTime CreatedAt { get; set; } 
        public List<PostSummaryViewModel> RecentPosts { get; set; } = new List<PostSummaryViewModel>();
    }
    public class MyProfileViewModel {
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public string? FullName { get; set; }
        public string? Bio { get; set; }
        public string? AvatarUrl { get; set; }
        public string? WebsiteUrl { get; set; }
        public bool IsPrivate { get; set; }
        public string? Gender { get; set; }
        public int FollowerCount { get; set; }
        public int FollowingCount { get; set; }
        public bool IsOwner { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public List<PostSummaryViewModel> Posts { get; set; } = new();
    }
    public class UserSummaryViewModel {
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? FullName { get; set; }
        public string? AvatarUrl { get; set; }
    }
}
