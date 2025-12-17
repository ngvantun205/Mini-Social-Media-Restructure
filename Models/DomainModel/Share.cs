namespace Mini_Social_Media.Models.DomainModel {
    public class Share {
        public int Id { get; set; }
        public int PostId { get; set; }
        public Post Post { get; set; }
        public string Caption { get; set; }
        public DateTime SharedAt { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
