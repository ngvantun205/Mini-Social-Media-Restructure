namespace Mini_Social_Media.DTO {
    public class FollowerDto {
        public int UserId { get; set; }
        public string UserName { get; set; } = "";
        public string? AvatarUrl { get; set; }
    }

    public class FollowingDto {
        public int UserId { get; set; }
        public string UserName { get; set; } = "";
        public string? AvatarUrl { get; set; }
    }
}
