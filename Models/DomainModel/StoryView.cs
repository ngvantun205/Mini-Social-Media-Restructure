using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mini_Social_Media.Models.DomainModel {
    public class StoryView {
        [Key]
        public int Id { get; set; }
        public int StoryId { get; set; }
        [ForeignKey(nameof(StoryId))]
        public Story Story { get; set; }
        public int ViewerId { get; set; }
        [ForeignKey(nameof(ViewerId))]
        public User Viewer { get; set; }
        public DateTime ViewedAt { get; set; } = DateTime.UtcNow;
    }
}
