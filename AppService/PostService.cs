using Mini_Social_Media.IAppService;

namespace Mini_Social_Media.AppService {
    public class PostService : IPostService {
        private readonly IPostRepository _postRepository;
        private readonly IUploadService _uploadService;
        public PostService(IPostRepository postRepository, IUploadService uploadService) {
            _postRepository = postRepository;
            _uploadService = uploadService;
        }
        public async Task<CreatePostDto> CreatePost(PostInputModel model, int userId) {
            var post = new Post {
                Caption = model.Caption,
                Location = model.Location,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };
            if (model.MediaFiles != null) {
                foreach (var file in model.MediaFiles) {
                    var url = await _uploadService.UploadAsync(file);

                    post.Medias.Add(new PostMedia {
                        Url = url,
                        MediaType = file.ContentType.StartsWith("video") ? "video" : "image"
                    });
                }
            }

            await _postRepository.AddAsync(post);

            return new CreatePostDto {
                PostId = post.PostId,
                MediaUrls = post.Medias.Select(x => x.Url).ToList()
            };
        }
        public async Task<PostDto?> GetByIdAsync(int postId) {
            var post = await _postRepository.GetByIdAsync(postId);
            return new PostDto {
            PostId = post.PostId,
            UserId = post.UserId,
            Caption = post.Caption,
            Location = post.Location,
            LikeCount = post.LikeCount,
            CommentCount = post.CommentCount
            };
        }

    }
}
