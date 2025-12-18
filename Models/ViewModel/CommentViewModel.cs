namespace Mini_Social_Media.Models.ViewModel {
    public class CommentViewModel {
        public int CommentId { get; set; }
        public int PostId { get; set; }
        public UserSummaryViewModel Owner { get; set; }
        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ReplyCount { get; set; }
        public int? ParentCommentId { get; set; }
        public string PostUsername { get; set; }     
        public ICollection<LikeViewModel> Likes { get; set; } = new List<LikeViewModel>();
        public ICollection<CommentViewModel> Replies { get; set; } = new List<CommentViewModel>();
    }
}
