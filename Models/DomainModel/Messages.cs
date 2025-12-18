using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mini_Social_Media.Models.DomainModel {
    public class Messages {
        [Key]
        public int Id { get; set; }
        public int ConversationId { get; set; }
        [ForeignKey("ConversationId")]
        public Conversations Conversation { get; set; }
        public int SenderId { get; set; }
        [ForeignKey("SenderId")]
        public User Sender { get; set; }
        public int ReceiverId { get; set; }
        [ForeignKey("ReceiverId")]
        public User Receiver { get; set; }
        public string? Content { get; set; }
        public string MessageType { get; set; } = "Text";
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
    }
}
