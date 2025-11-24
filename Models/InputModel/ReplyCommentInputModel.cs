namespace Mini_Social_Media.Models.InputModel {
    public class ReplyCommentInputModel {
        public int PostId { get; set; }
        public int ParentCommentId { get; set; }
        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
