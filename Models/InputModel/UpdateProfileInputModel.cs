namespace Mini_Social_Media.Models.InputModel {
    public class UpdateProfileInputModel {
        public string FullName { get; set; } = "";
        public string Bio { get; set; } = "";
        public string AvatarUrl { get; set; } = ""; 
        public string WebsiteUrl { get; set; } = "";
        public bool IsPrivate { get; set; }
    }

}
