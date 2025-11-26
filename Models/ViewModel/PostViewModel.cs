using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mini_Social_Media.Models.ViewModel {
    public class PostViewModel {
        public int PostId { get; set; }
        public UserSummaryViewModel Owner { get; set; }
        public string? Caption { get; set; }
        public string? Location { get; set; }
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsLiked { get; set; }    
        public List<PostMediaViewModel> Medias { get; set; } = new List<PostMediaViewModel>();
        public List<CommentViewModel> Comments { get; set; } = new List<CommentViewModel>();
        public string? Hashtags { get; set; }
    }
    public class PostSummaryViewModel {
        public int PostId { get; set; }
        public int UserId { get; set; }
        public string? MediaUrl { get; set; }
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
    }
}
