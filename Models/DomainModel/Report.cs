using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mini_Social_Media.Models.DomainModel {
    public class Report {
        [Key]
        public int ReportId { get; set; }
        public int ReporterId { get; set; }
        [ForeignKey(nameof(ReporterId))]
        public User Reporter { get; set; }
        public string? Content { get; set; }
        public string Type { get; set; }
        public int EntityId { get; set; }
        public bool IsExecuted { get; set; }    
        public DateTime CreatedAt { get; set; } 
    }
}
