namespace Mini_Social_Media {
    public class CommentDto {
        public int CommentId { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? FullName { get; set; }
        public string? UserAvatarUrl { get; set; }
        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<ReplyCommentDto> Replies { get; set; } = new();
    }

    public class ReplyCommentDto {
        public int CommentId { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? FullName { get; set; }   
        public string? UserAvatarUrl { get; set; }
        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ParentCommentId { get; set; }    
    }
}
