namespace Mini_Social_Media.AppService {
    public class PostService : IPostService {
        private readonly IPostRepository _postRepository;
        private readonly IUploadService _uploadService;
        private readonly IHashtagRepository _hashtagRepository; 
        public PostService(IPostRepository postRepository, IUploadService uploadService, IHashtagRepository hashtagRepository) {
            _postRepository = postRepository;
            _uploadService = uploadService;
            _hashtagRepository = hashtagRepository;
        }
        public async Task<CreatePostDto> CreatePost(PostInputModel model, int userId) {

            var post = new Post {
                UserId = userId,
                Caption = model.Caption,
                Location = model.Location,
                CreatedAt = DateTime.UtcNow
            };
            if(model.Hashtags != null) {
                var hashtags = model.Hashtags.Split(' ');
                foreach(var tag in hashtags) {
                    if(await _hashtagRepository.IsExist(tag)) {
                        var existingTag = await _hashtagRepository.GetByNameAsync(tag);
                        existingTag.UsageCount += 1;
                        post.PostHashtags.Add(new PostHashtag {
                            HashtagId = existingTag.HashtagId
                        });
                        Console.WriteLine("📌 Thêm hashtag có sẵn vào Post → " + tag);
                    } else {
                        var newTag = new Hashtag {
                            HashtagName = tag,
                            CreatedAt = DateTime.UtcNow,
                            UsageCount = 1
                        };
                        await _hashtagRepository.AddAsync(newTag);
                        post.PostHashtags.Add(new PostHashtag {
                            Hashtag = newTag
                        });
                        Console.WriteLine("📌 Tạo và thêm hashtag mới vào Post → " + tag);
                    }
                }
            }
            if (model.MediaFiles != null) {
                foreach (var file in model.MediaFiles) {
                    var url = await _uploadService.UploadAsync(file);

                    if (string.IsNullOrEmpty(url)) {
                        Console.WriteLine("❌ Không thể upload file: " + file.FileName);
                        continue;
                    }

                    Console.WriteLine("📌 Thêm Media vào Post → URL: " + url);

                    post.Medias.Add(new PostMedia {
                        Url = url,
                        MediaType = file.ContentType.StartsWith("video") ? "video" : "image"
                    });
                }
            }

            Console.WriteLine("📌 Tổng số media được thêm: " + post.Medias.Count);

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
                CreatedAt = post.CreatedAt,
                LikeCount = post.LikeCount,
                CommentCount = post.CommentCount,
                MediaUrls = post.Medias.Select(x => x.Url).ToList(),
                UserName = post.User?.UserName,
            };
        }

    }
}
