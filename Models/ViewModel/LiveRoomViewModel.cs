namespace Mini_Social_Media.Models.ViewModel {
    public class LiveRoomViewModel {
        public int RoomId { get; set; }
        public string Title { get; set; }
        public string LiveKitUrl { get; set; }
        public string Token { get; set; }     
        public bool IsHost { get; set; }   
        public string StreamerName { get; set; }
        public string StreamerAvatar { get; set; }
    }
}
