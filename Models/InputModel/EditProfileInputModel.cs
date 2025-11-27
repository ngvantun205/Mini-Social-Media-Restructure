namespace Mini_Social_Media.Models.InputModel {
    public class EditProfileInputModel {
        public string? FullName { get; set; }
        public string? Bio { get; set; }
        public IFormFile? AvatarUrl { get; set; }
        public string? WebsiteUrl { get; set; }
        public string? Gender { get; set; }
    }
}
