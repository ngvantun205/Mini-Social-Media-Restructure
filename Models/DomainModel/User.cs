using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mini_Social_Media.Models.DomainModel {
    public class User : IdentityUser<int> {
        public string? FullName { get; set; }
        public string? Bio { get; set; }
        public string? AvatarUrl { get; set; }
        public string? WebsiteUrl { get; set; }
        public bool IsPrivate { get; set; } = false;
        public string? Gender { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int PostId { get; set; }
        [ForeignKey(nameof(PostId))]
        public ICollection<Post> Posts { get; set; } = new List<Post>();
        public int CommentId { get; set; }
        [ForeignKey(nameof(CommentId))]
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public int LikeId { get; set; }
        [ForeignKey(nameof(LikeId))]
        public ICollection<Like> Likes { get; set; } = new List<Like>();
        public ICollection<Follow> Followers { get; set; } = new List<Follow>();
        public ICollection<Follow> Following { get; set; } = new List<Follow>();
        public ICollection<Notifications> ReceivedNotifications { get; set; } = new List<Notifications>();
        public ICollection<Notifications> SentNotifications { get; set; } = new List<Notifications>();
    }
}
