using Microsoft.Extensions.Hosting;
using Mini_Social_Media.Models.DomainModel;
using System.Text.RegularExpressions;

namespace Mini_Social_Media.AppService {
    public class PostService : IPostService {
        private readonly IPostRepository _postRepository;
        private readonly IUploadService _uploadService;
        private readonly IHashtagRepository _hashtagRepository;
        private readonly IPostMediaRepository _postMediaRepository;
        private readonly ILikeRepository _likeRepository;
        private readonly ICommentRepository _commentRepository;

        public PostService(IPostRepository postRepository, IUploadService uploadService, IHashtagRepository hashtagRepository, IPostMediaRepository postMediaRepository, ILikeRepository likeRepository, ICommentRepository commentRepository) {
            _postRepository = postRepository;
            _uploadService = uploadService;
            _hashtagRepository = hashtagRepository;
            _postMediaRepository = postMediaRepository;
            _likeRepository = likeRepository;
            _commentRepository = commentRepository;
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
                    }
                }
            }
            if (model.MediaFiles != null) {
                foreach (var file in model.MediaFiles) {
                    var url = await _uploadService.UploadAsync(file);

                    if (string.IsNullOrEmpty(url)) {
                        continue;
                    }
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

        public async Task<PostDto?> GetByIdAsync(int postId, int userId) {
            var post = await _postRepository.GetByIdAsync(postId);
            if (post == null)
                return null;

            var hashtagNames = post.PostHashtags.Select(ph => ph.Hashtag.HashtagName).ToList();
            var comments = await _commentRepository.GetCommentsByPostIdAsync(postId);
            return new PostDto {
                PostId = post.PostId,
                Owner = new UserSummaryDto() {UserName = post.User?.UserName, FullName = post.User ?.FullName, AvatarUrl = post.User ?.AvatarUrl, UserId = userId  },
                Caption = post.Caption,
                Location = post.Location,
                CreatedAt = post.CreatedAt,
                LikeCount = post.LikeCount,
                IsLiked = post.Likes.Any(l => l.UserId == userId),
                CommentCount = post.CommentCount,
                MediaUrls = post.Medias.Select(m => new PostMediaDto { Url = m.Url, MediaType = m.MediaType }).ToList(),
                Hashtags = string.Join(" ", hashtagNames) ,
                Comments = comments.Select(c => new CommentDto {
                    CommentId = c.CommentId,
                    PostId = c.PostId,
                    UserId = c.UserId,
                    UserName = c.User?.UserName,
                    FullName = c.User?.FullName,
                    UserAvatarUrl = c.User?.AvatarUrl,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt
                }).ToList()
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
                Owner = new UserSummaryDto() { UserName = post.User?.UserName, FullName = post.User?.FullName, AvatarUrl = post.User?.AvatarUrl, UserId = userId },
                Caption = post.Caption,
                Location = post.Location,
                CreatedAt = post.CreatedAt,
                LikeCount = post.LikeCount,
                CommentCount = post.CommentCount,
                MediaUrls = post.Medias.Select(m => new PostMediaDto { Url = m.Url, MediaType = m.MediaType }).ToList(),
                Hashtags = string.Join(' ', post.PostHashtags.Select(x => x.Hashtag.HashtagName))
            };
        }
        public async Task<bool> DeletePostAsync(int postId, int userId) {
            var post = await _postRepository.GetByIdAsync(postId);
            if (post == null || post.UserId != userId)
                return false;

            foreach (var ph in post.PostHashtags) {
                ph.Hashtag.UsageCount = Math.Max(0, ph.Hashtag.UsageCount - 1);
            }
            await _postRepository.DeleteAsync(postId);
            return true;
        }

        public async Task<IEnumerable<PostDto>> GetPostsPagedAsync(int page, int pageSize, int userId) {
            var posts = await _postRepository.GetPostsPagedAsync(page, pageSize);

            return posts.Select(p => new PostDto {
                PostId = p.PostId,
                Owner = new UserSummaryDto() { UserName = p.User?.UserName, FullName = p.User?.FullName, AvatarUrl = p.User?.AvatarUrl, UserId = userId },
                Caption = p.Caption,
                CreatedAt = p.CreatedAt,
                MediaUrls = p.Medias.Select(m =>  new PostMediaDto() {Url = m.Url, MediaType = m.MediaType }).ToList(),
                LikeCount = p.LikeCount,
                CommentCount = p.CommentCount,
                IsLiked = p.Likes.Any(l => l.UserId == userId),
                Hashtags = string.Join(" ", p.PostHashtags.Select(ph => ph.Hashtag.HashtagName))
            }).ToList();
        }
    }
}