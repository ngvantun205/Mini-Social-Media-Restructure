namespace Mini_Social_Media.Models.ViewModel {
    public class EditProfileViewModel {
        public string? UserName { get; set; }   
        public string? FullName { get; set; }
        public string? Bio { get; set; }
        public string? AvatarUrl { get; set; }
        public string? WebsiteUrl { get; set; }
        public string? Gender { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
