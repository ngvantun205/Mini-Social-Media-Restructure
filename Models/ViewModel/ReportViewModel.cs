namespace Mini_Social_Media.Models.ViewModel {
    public class ReportViewModel {
        public int ReportId { get; set; }
        public int ReporterId { get; set; } 
        public string? Content { get; set; }
        public string Type { get; set; }
        public int EntityId { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsExecuted { get; set; }    
    }
}
