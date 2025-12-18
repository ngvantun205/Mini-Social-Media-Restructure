namespace Mini_Social_Media.Models.ViewModel {
    public class MessageViewModel {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
        public string SenderName { get; set; }
        public string SenderAvatar { get; set; }
        public string MessageType { get; set; }
    }
}
