namespace Mini_Social_Media.Models.ViewModel {
    public class LikeViewModel {
        public UserSummaryViewModel Owner { get; set; }
        public int PostId { get; set; }
        public string PostUsername { get; set; }     
        public DateTime CreatedAt { get; set; } 
    }
}
