using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mini_Social_Media.Models.DomainModel {
    public class LiveChatMessage {
        [Key]
        public int Id { get; set; }

        public int LiveStreamId { get; set; }

        [ForeignKey("LiveStreamId")]
        public virtual LiveStream LiveStream { get; set; }

        public int UserId { get; set; } // Người chat

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [Required]
        [StringLength(500)]
        public string Content { get; set; }

        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}
