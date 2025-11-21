namespace Mini_Social_Media.Models.InputModel {
    public class PostInputModel {
        public string? Caption { get; set; }
        public string? Location { get; set; }
        public List<IFormFile>? MediaFiles { get; set; }
        public List<string>? Hashtags { get; set; }
    }
}
