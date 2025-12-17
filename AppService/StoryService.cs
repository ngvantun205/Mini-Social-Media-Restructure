

namespace Mini_Social_Media.AppService {
    public class StoryService  : IStoryService {
        private readonly IStoryRepository _storyRepository;
        private readonly IStoryArchiveRepository _archiveRepository;
        private readonly IUploadService _uploadService;
        public StoryService(IStoryRepository storyRepository, IStoryArchiveRepository storyArchiveRepository, IUploadService uploadService) {
            _storyRepository = storyRepository;
            _archiveRepository = storyArchiveRepository;
            _uploadService = uploadService;
        }

        public async Task<StoryViewModel> AddStory(StoryInputModel inputModel, int userId) {
            var url = await _uploadService.UploadAsync(inputModel.StoryMedia);
            var story = new Story() {
                Caption = inputModel.Caption,
                MediaUrl = url,
                MediaType = url.EndsWith(".mp4") ? "video" : "image",
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddHours(24),
                UserId = userId,
            };
            await _storyRepository.AddAsync(story);
            var newstr = await _storyRepository.GetByIdAsync(story.Id);
            return new StoryViewModel() {
                Caption = newstr.Caption,
                MediaUrl = newstr.MediaUrl,
                MediaType = newstr.MediaType,
                CreatedAt = newstr.CreatedAt,
                StoryId = newstr.Id,
                Owner = new UserSummaryViewModel() { UserId = newstr.User.Id, AvatarUrl = newstr.User.AvatarUrl, FullName = newstr.User.FullName, UserName = newstr.User.UserName }
            };
        }

        public async Task DeleteArchive(int archiveId) {
           await _archiveRepository.DeleteAsync(archiveId);
        }

        public async Task DeleteStory(int storyId) {
            await _storyRepository.DeleteAsync(storyId);
        }
        public async Task<IEnumerable<UserStoryViewModel>> GetCurrentStories(int userId) {
            var rawStories = await _storyRepository.GetFriendsStories(userId);

            if (rawStories == null || !rawStories.Any())
                return new List<UserStoryViewModel>();

            var groupedStories = rawStories
                .GroupBy(s => s.UserId) 
                .Select(g => {
                    var user = g.First().User; 
                    return new UserStoryViewModel {
                        UserId = user.Id,
                        UserName = user.UserName,
                        AvatarUrl = user.AvatarUrl,
                        HasUnseenStories = true, 
                        Stories = g.Select(s => new StoryViewModel {
                            Owner = new UserSummaryViewModel() { UserId = s.User.Id, FullName = s.User.FullName, AvatarUrl = s.User.AvatarUrl, UserName = s.User.UserName },
                            StoryId = s.Id,
                            MediaUrl = s.MediaUrl,
                            MediaType = s.MediaType,
                            Caption = s.Caption,
                            CreatedAt = s.CreatedAt
                        }).OrderBy(s => s.CreatedAt).ToList() 
                    };
                })
                .ToList();

            return groupedStories;
        }

        public async Task<IEnumerable<StoryArchiveViewModel>> GetUserStoryArchives(int userId) {
            var archives =  await _archiveRepository.GetUserStoryArchive(userId);
            return archives.Select(x => new StoryArchiveViewModel() {
                Caption = x.Caption,
                MediaUrl = x.MediaUrl,
                MediaType = x.MediaType,
                CreatedAt = x.CreatedAt,
                StoryArchiveId = x.Id,
                Owner = new UserSummaryViewModel() {UserId = x.User.Id, AvatarUrl = x.User.AvatarUrl, FullName = x.User.FullName, UserName = x.User.UserName }
            }).ToList();
        }
    }
}
