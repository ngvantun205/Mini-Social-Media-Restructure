using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mini_Social_Media.Models.DomainModel {
    public class Post {
        [Key]
        public int PostId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }
        public int UserId { get; set; }
        public string? Caption { get; set; }
        public string? Location { get; set; }
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<PostMedia> Medias { get; set; } = new List<PostMedia>();
        public List<Comment>? Comments { get; set; } = new List<Comment>();
        public List<Like>? Likes { get; set; } = new List<Like>();
        public List<PostHashtag>? PostHashtags { get; set; } = new List<PostHashtag>();
    }
}
