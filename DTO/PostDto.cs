namespace Mini_Social_Media.DTO {
    public class PostSummaryDto {
        public int PostId { get; set; }
        public int UserId { get; set; }
        public string? MediaUrl { get; set; }    
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
    }
    public class PostDto {
        public int PostId { get; set; }
        public UserSummaryDto Owner { get; set; }
        public string? Caption { get; set; }
        public string? Location { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<PostMediaDto>? MediaUrls { get; set; }
        public List<CommentDto> Comments { get; set; } = new();
        public string? Hashtags { get; set; } 
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
        public bool IsLiked { get; set; }
    }
    public class PostMediaDto {
        public string Url { get; set; }
        public string MediaType { get; set; }
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
