namespace Mini_Social_Media.DTO {
    public class UserFollowDto {
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? AvatarUrl { get; set; }
    }
    public class FollowDto {
        public string? ErrorMessage { get; set; }   
    }
}
