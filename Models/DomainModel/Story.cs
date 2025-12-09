using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mini_Social_Media.Models.DomainModel {
    public class Story {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
        public string MediaUrl { get; set; }
        public string MediaType { get; set; }
        public string Caption { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; set; }
        public int ViewCount { get; set; } = 0;
        public ICollection<StoryView> Views { get; set; } = new List<StoryView>();

    }
}
