using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mini_Social_Media.Models.DomainModel {
    public class Like {
        [Key]
        public int LikeId { get; set; }
        public int PostId { get; set; }
        [ForeignKey("PostId")]
        public Post? Post { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
