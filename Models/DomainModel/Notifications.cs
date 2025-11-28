using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mini_Social_Media.Models.DomainModel {
    public class Notifications {
        [Key]
        public int NotiId { get; set; }
        public int ReceiverId { get; set; }
        [ForeignKey(nameof(ReceiverId))]
        public User? Receiver { get; set; }
        public int ActorId { get; set; }
        [ForeignKey(nameof(ActorId))]
        public User? Actor { get; set; }
        public string? Type { get; set; }
        public int EntityId { get; set; }
        public bool IsRead { get; set; }
        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; } 
    }
}
