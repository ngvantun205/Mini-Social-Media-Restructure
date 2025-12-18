namespace Mini_Social_Media.Models.InputModel {
    public class SendMessageInputModel {
        public int ReceiverId { get; set; }
        public string Content { get; set; }
    }
    public class SendImgOrVoiceInputModel {
        public int ReceiverId { get; set; }
        public IFormFile MessageFile { get; set; }
    }
}
