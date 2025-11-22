using Mini_Social_Media.Models.DomainModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mini_Social_Media.Models {
    public class Comment {
        [Key]
        public int CommentId { get; set; }
        public int PostId { get; set; }
        [ForeignKey("PostId")]
        public Post? Post { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }
        public string? Content { get; set; }
        public int ParentCommentId { get; set; }
        public Comment? ParentComment { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<Comment>? Replies { get; set; }
    }
}
