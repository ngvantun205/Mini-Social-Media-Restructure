namespace Mini_Social_Media.Models.ViewModel {
    public class LiveChatMessageViewModel {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserAvatar { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
    }
}
