namespace Mini_Social_Media.Models.ViewModel {
    public class CommentViewModel {
        public int CommentId { get; set; }
        public string? UserName { get; set; }
        public string? FullName { get; set; }    
        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? UserAvatarUrl { get; set; } 
        public int LikeCount { get; set; }
        public ICollection<LikeViewModel> Likes { get; set; } = new List<LikeViewModel>();
        public ICollection<CommentViewModel> Replies { get; set; } = new List<CommentViewModel>();
    }
}
