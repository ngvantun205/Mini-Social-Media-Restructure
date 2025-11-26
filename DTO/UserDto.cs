namespace Mini_Social_Media.DTO {
    public class UserSummaryDto {
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? FullName { get; set; }
        public string? AvatarUrl { get; set; }
    }

    public class UserProfileDto {
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? FullName { get; set; }
        public string? Bio { get; set; }
        public string? AvatarUrl { get; set; }
        public string? WebsiteUrl { get; set; }
        public bool IsPrivate { get; set; }
        public string? Gender { get; set; }
        public int FollowersCount { get; set; }
        public int FollowingCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsFollowing { get; set; }
        public List<PostSummaryDto> Posts { get; set; } = new();
    }
    public class MyProfileDto {
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public string? FullName { get; set; }
        public string? Bio { get; set; }
        public string? AvatarUrl { get; set; }
        public string? WebsiteUrl { get; set; }
        public bool IsPrivate { get; set; }
        public string? Gender { get; set; }
        public int FollowersCount { get; set; }
        public int FollowingCount { get; set; }
        public bool IsOwner { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public List<PostSummaryDto> Posts { get; set; } = new();
    }
}
