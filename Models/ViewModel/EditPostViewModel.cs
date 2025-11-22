namespace Mini_Social_Media.Models.ViewModel {
    public class EditPostViewModel {
        public int PostId { get; set; }
        public string? Caption { get; set; }
        public string? Location { get; set; }
        public string? Hashtags { get; set; }
        public List<string> MediaFiles { get; set; } = new();
    }
}
