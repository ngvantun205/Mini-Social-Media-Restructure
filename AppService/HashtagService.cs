namespace Mini_Social_Media.AppService {
    public class HashtagService : IHashtagService {
        private readonly IHashtagRepository _hastagRepository;
        public HashtagService(IHashtagRepository repository) {
            _hastagRepository = repository;
        }
        public async Task<IEnumerable<Hashtag>> GetTopHashtag() => await _hastagRepository.GetTopHashtag();
    }
}
