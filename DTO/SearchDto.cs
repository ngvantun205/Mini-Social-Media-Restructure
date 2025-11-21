namespace Mini_Social_Media.DTO {
    public class SearchUserDto {
        public int UserId { get; set; }
        public string UserName { get; set; } = "";
        public string? AvatarUrl { get; set; }
        public string? FullName { get; set; }
    }

    public class SearchPostDto {
        public int PostId { get; set; }
        public List<string> MediaUrls { get; set; } = new();
    }
}
