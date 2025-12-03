using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mini_Social_Media.Models.DomainModel {
    public class Follow {
        [Key]
        public int FollowId { get; set; }
        public int FollowerId { get; set; }
        public int FolloweeId { get; set; }
        public DateTime CreatedAt { get; set; }
        [ForeignKey(nameof(FollowerId))]
        public User? Follower { get; set; }
        [ForeignKey(nameof(FolloweeId))]
        public User? Followee { get; set; }
    }
}
