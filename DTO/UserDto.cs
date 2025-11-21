namespace Mini_Social_Media.DTO {
    public class UserSummaryDto {
        public int UserId { get; set; }
        public string UserName { get; set; } = "";
        public string? FullName { get; set; }
        public string? AvatarUrl { get; set; }
    }

    public class UserProfileDto {
        public int UserId { get; set; }
        public string UserName { get; set; } = "";
        public string? FullName { get; set; }
        public string? Bio { get; set; }
        public string? AvatarUrl { get; set; }
        public string? WebsiteUrl { get; set; }
        public bool IsPrivate { get; set; }

        public int FollowerCount { get; set; }
        public int FollowingCount { get; set; }

        public bool IsOwner { get; set; }
        public bool IsFollowing { get; set; }

        public List<PostSummaryDto> Posts { get; set; } = new();
    }
}
