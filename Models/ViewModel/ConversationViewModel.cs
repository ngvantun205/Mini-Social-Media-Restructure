namespace Mini_Social_Media.Models.ViewModel {
    public class ConversationViewModel {
        public int ConversationId { get; set; }
        public int PartnerId { get; set; }
        public string PartnerName { get; set; }
        public string PartnerAvatar { get; set; }
        public string LastMessage { get; set; }
        public DateTime LastMessageTime { get; set; }
        public bool IsRead { get; set; }
    }
}
