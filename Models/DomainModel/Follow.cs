using System.ComponentModel.DataAnnotations;

namespace Mini_Social_Media.Models.DomainModel {
    public class Follow {
        [Key]
        public int FollowId { get; set; }
        public int FollowerId { get; set; }
        public int FolloweeId { get; set; }
        public DateTime CreatedAt { get; set; }
        public User? Follower { get; set; }
        public User? Followee { get; set; }
    }
}
