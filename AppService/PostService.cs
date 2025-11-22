namespace Mini_Social_Media.AppService {
    public class PostService : IPostService {
        private readonly IPostRepository _postRepository;
        private readonly IUploadService _uploadService;
        private readonly IHashtagRepository _hashtagRepository;
        private readonly IPostMediaRepository _postMediaRepository;
        public PostService(IPostRepository postRepository, IUploadService uploadService, IHashtagRepository hashtagRepository, IPostMediaRepository postMediaRepository) {
            _postRepository = postRepository;
            _uploadService = uploadService;
            _hashtagRepository = hashtagRepository;
            _postMediaRepository = postMediaRepository;
        }
        public async Task<CreatePostDto> CreatePost(PostInputModel model, int userId) {

            var post = new Post {
                UserId = userId,
                Caption = model.Caption,
                Location = model.Location,
                CreatedAt = DateTime.UtcNow
            };
            if (model.Hashtags != null) {
                var hashtags = model.Hashtags.Split(' ');
                foreach (var tag in hashtags) {
                    if (await _hashtagRepository.IsExist(tag)) {
                        var existingTag = await _hashtagRepository.GetByNameAsync(tag);
                        existingTag.UsageCount += 1;
                        post.PostHashtags.Add(new PostHashtag {
                            HashtagId = existingTag.HashtagId
                        });
                        Console.WriteLine("📌 Thêm hashtag có sẵn vào Post → " + tag);
                    }
                    else {
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
            if (post == null) {
                return null;
            }
            var hashtagNames = post.PostHashtags
                .Select(ph => ph.Hashtag.HashtagName)
                .ToList();
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
                Hashtags = string.Join(' ', hashtagNames)
            };
        }
        public async Task<PostDto?> EditPostAsync(EditPostInputModel model, int userId) {
            var post = await _postRepository.GetByIdAsync(model.PostId);
            if (post == null || post.UserId != userId) {
                return null;
            }
            post.Caption = model.Caption;
            post.Location = model.Location;

            if (model.Hashtags != null) {
                var hashtags = model.Hashtags.Split(' ');
                foreach (var tag in hashtags) {
                    if (await _hashtagRepository.IsExist(tag) && !post.PostHashtags.Select(x => x.Hashtag.HashtagName).Contains(tag)) {
                        var existingTag = await _hashtagRepository.GetByNameAsync(tag);
                        existingTag.UsageCount += 1;
                        post.PostHashtags.Add(new PostHashtag {
                            HashtagId = existingTag.HashtagId
                        });
                        Console.WriteLine("📌 Thêm hashtag có sẵn vào Post → " + tag);
                    }
                    else if (!await _hashtagRepository.IsExist(tag)) {
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

            if (model.RemovedMedia != null) {
                var updatedMedia = model.ExistingMedia
                    .Where(url => !model.RemovedMedia.Contains(url))
                    .ToList();
                var removedMedia = post.Medias
                    .Where(pm => model.RemovedMedia.Contains(pm.Url))
                    .ToList();
                await _postMediaRepository.RemoveRangeAsync(removedMedia);

                if (model.NewMediaFiles != null) {
                    foreach (var file in model.NewMediaFiles) {
                        var url = await _uploadService.UploadAsync(file);
                        if (url != null)
                            updatedMedia.Add(url);
                    }
                }
                post.Medias = updatedMedia.Select(url => new PostMedia {
                    Url = url,
                    MediaType = url.EndsWith(".mp4") ? "video" : "image"
                }).ToList();
            }

            await _postRepository.UpdateAsync(post);

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
                Hashtags = string.Join(' ', post.PostHashtags.Select(x => x.Hashtag.HashtagName))
            };
        }
        public async Task<bool> DeletePostAsync(int postId, int userId) {
            var post = await _postRepository.GetByIdAsync(postId);
            if (post == null || post.UserId != userId) {
                return false;
            }
            await _postRepository.DeleteAsync(postId);
            return true;
        }
    }
}
