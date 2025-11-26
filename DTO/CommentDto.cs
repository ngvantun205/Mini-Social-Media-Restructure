namespace Mini_Social_Media {
    public class CommentDto {
        public int PostId { get; set; }
        public int CommentId { get; set; }
        public int? ParentCommentId { get; set; }
        public UserSummaryDto? Owner { get; set; }
        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ReplyCount { get; set; }  
        public List<CommentDto> Replies { get; set; } = new();
    }
}
