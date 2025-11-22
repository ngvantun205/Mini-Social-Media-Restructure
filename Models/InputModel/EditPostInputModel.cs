namespace Mini_Social_Media.Models.InputModel {
    public class EditPostInputModel {
        public int PostId { get; set; } 
        public string? Caption { get; set; }
        public string? Location { get; set; }
        public IFormFileCollection? MediaFiles { get; set; }
        public string? Hashtags { get; set; }
    }
}
