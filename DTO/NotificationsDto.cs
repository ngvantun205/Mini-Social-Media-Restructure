namespace Mini_Social_Media.DTO {
    public class NotificationsDto {
        public int NotiId { get; set; }
        public string ActorName { get; set; }
        public string ActorAvatar { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public int PostId { get; set; }
        public bool IsRead { get; set; }
        public string? Type { get; set; }    
    }
}
