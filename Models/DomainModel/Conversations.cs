using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mini_Social_Media.Models.DomainModel {
    public class Conversations {
        [Key]
        public int Id { get; set; } 
        public int User1Id { get; set; }
        [ForeignKey("User1Id")]
        public User User1 { get; set; }
        public int User2Id { get; set; }
        [ForeignKey("User2Id")]
        public User User2 { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int? LatestMessageId { get; set; }
        [ForeignKey("LatestMessageId")]
        public Messages? LatestMessage { get; set; }
        public ICollection<Messages> Messages { get; set; }
    }
}
