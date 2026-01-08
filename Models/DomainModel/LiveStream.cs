using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mini_Social_Media.Models.DomainModel {
    public enum LiveStreamStatus {
        OnAir = 0, 
        Ended = 1,
        Banned = 2
    }
    public class LiveStream {
        [Key]
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        [Required(ErrorMessage = "Tiêu đề không được để trống")]
        [StringLength(200)]
        public string Title { get; set; }
        public string? ThumbnailUrl { get; set; } 
        [Required]
        public string ExternalRoomId { get; set; }
        public LiveStreamStatus Status { get; set; } = LiveStreamStatus.OnAir;
        public int ViewCount { get; set; } = 0;
        public int TotalViews { get; set; } = 0;
        public int LikeCount { get; set; } = 0;
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime? EndedAt { get; set; }
        public virtual ICollection<LiveChatMessage> Messages { get; set; } = new List<LiveChatMessage>();
    }
}
