using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Mini_Social_Media.Models.DomainModel {
    public class User : IdentityUser<int> {
        public string FullName { get; set; } = "";
        public string Bio { get; set; } = "";
        public string AvatarUrl { get; set; } = "";
        public string WebsiteUrl { get; set; } = "";
        public bool IsPrivate { get; set; } = false; 
        public DateTime CreatedAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public ICollection<Post> Posts { get; set; } = new List<Post>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Like> Likes { get; set; } = new List<Like>();
        public ICollection<Follow> Followers { get; set; } = new List<Follow>();
        public ICollection<Follow> Following { get; set; } = new List<Follow>();
    }
}
