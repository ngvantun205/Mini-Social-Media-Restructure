namespace Mini_Social_Media.DTO {
    public class RegisterDto {
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
    }

    public class LoginDto {
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? Token { get; set; }
    }
}
