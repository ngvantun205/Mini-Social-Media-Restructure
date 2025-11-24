namespace Mini_Social_Media.DTO {
    public class PostSummaryDto {
        public int PostId { get; set; }
        public int UserId { get; set; }
        public List<string>? MediaUrls { get; set; }    
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
    }
    public class PostDto {
        public int PostId { get; set; }
        public int UserId { get; set; }
        public string? FullName { get; set; }
        public string? UserName { get; set; }
        public string? UserAvatar { get; set; }
        public string? Caption { get; set; }
        public string? Location { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<string>? MediaUrls { get; set; }
        public string? Hashtags { get; set; } 
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
        public bool IsLiked { get; set; }
    }
    public class CreatePostResultDto {
        public int PostId { get; set; }
        public DateTime CreatedAt { get; set; }

    }
    public class CreatePostDto {
        public int PostId { get; set; } 
        public int UserId { get; set; }
        public string? Caption { get; set; }
        public List<string>? MediaUrls { get; set; }
    }
}
