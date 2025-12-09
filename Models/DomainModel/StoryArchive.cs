using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mini_Social_Media.Models.DomainModel {
    public class StoryArchive {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
        public string MediaUrl { get; set; }
        public string MediaType { get; set; }
        public string? Caption { get; set; }
        public DateTime CreatedAt { get; set; } 
        public DateTime ArchivedAt { get; set; } = DateTime.UtcNow;
        public int ViewCount { get; set; } 
    }
}
