using Mini_Social_Media.Models.DomainModel;

namespace Mini_Social_Media.AppService {
    public class PostService : IPostService {
        private readonly IPostRepository _postRepository;
        private readonly IUploadService _uploadService;
        private readonly IHashtagRepository _hashtagRepository;
        private readonly IPostMediaRepository _postMediaRepository;
        private readonly ILikeRepository _likeRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly IShareRepository _shareRepository;

        public PostService(IPostRepository postRepository, IUploadService uploadService, IHashtagRepository hashtagRepository, IPostMediaRepository postMediaRepository, ILikeRepository likeRepository, ICommentRepository commentRepository, IShareRepository shareRepository) {
            _postRepository = postRepository;
            _uploadService = uploadService;
            _hashtagRepository = hashtagRepository;
            _postMediaRepository = postMediaRepository;
            _likeRepository = likeRepository;
            _commentRepository = commentRepository;
            _shareRepository = shareRepository;
        }

        public async Task<PostViewModel> CreatePost(PostInputModel model, int userId) {

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
            return new PostViewModel {
                PostId = post.PostId,
                Owner = new UserSummaryViewModel() { UserName = post.User?.UserName, FullName = post.User?.FullName, AvatarUrl = post.User?.AvatarUrl, UserId = userId },
                Caption = post.Caption,
                Location = post.Location,
                CreatedAt = post.CreatedAt,
                LikeCount = post.LikeCount,
                CommentCount = post.CommentCount,
                Medias = post.Medias.Select(m => new PostMediaViewModel { Url = m.Url, MediaType = m.MediaType }).ToList(),
                Hashtags = string.Join(' ', post.PostHashtags.Select(x => x.Hashtag.HashtagName))
            };
        }

        public async Task<PostViewModel?> GetByIdAsync(int postId, int userId) {
            var post = await _postRepository.GetByIdAsync(postId);
            if (post == null)
                return null;
            var hashtagNames = post.PostHashtags.Select(ph => ph.Hashtag.HashtagName).ToList();
            var comments = await _commentRepository.GetCommentsByPostIdAsync(postId);
            return new PostViewModel {
                PostId = post.PostId,
                Owner = new UserSummaryViewModel() { UserName = post.User?.UserName, FullName = post.User?.FullName, AvatarUrl = post.User?.AvatarUrl, UserId = post.User.Id },
                Caption = post.Caption,
                Location = post.Location,
                CreatedAt = post.CreatedAt,
                LikeCount = post.LikeCount,
                IsLiked = post.Likes.Any(l => l.UserId == userId),
                CommentCount = post.CommentCount,
                Medias = post.Medias.Select(m => new PostMediaViewModel { Url = m.Url, MediaType = m.MediaType }).ToList(),
                Hashtags = string.Join(" ", hashtagNames),
                Comments = comments.Select(c => new CommentViewModel {
                    CommentId = c.CommentId,
                    Owner = new UserSummaryViewModel() { UserId = c.User.Id, UserName = c.User.UserName, FullName = c.User.FullName, AvatarUrl = c.User.AvatarUrl },
                    Content = c.Content,
                    ParentCommentId = c.ParentCommentId,
                    CreatedAt = c.CreatedAt,
                    ReplyCount = c.ReplyCount
                }).OrderBy(c => c.CreatedAt).ToList() ?? new List<CommentViewModel>()
            };
        }

        public async Task<PostViewModel?> EditPostAsync(EditPostInputModel model, int userId) {
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

            return new PostViewModel {
                PostId = post.PostId,
                Owner = new UserSummaryViewModel() { UserName = post.User?.UserName, FullName = post.User?.FullName, AvatarUrl = post.User?.AvatarUrl, UserId = userId },
                Caption = post.Caption,
                Location = post.Location,
                CreatedAt = post.CreatedAt,
                LikeCount = post.LikeCount,
                CommentCount = post.CommentCount,
                Medias = post.Medias.Select(m => new PostMediaViewModel { Url = m.Url, MediaType = m.MediaType }).ToList(),
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

        public async Task<IEnumerable<PostViewModel>> GetPostsPagedAsync(int page, int pageSize, int userId) {
            var posts = await _postRepository.GetPostsPagedAsync(page, pageSize);

            return posts.Select(p => new PostViewModel {
                PostId = p.PostId,
                Owner = new UserSummaryViewModel() { UserName = p.User?.UserName, FullName = p.User?.FullName, AvatarUrl = p.User?.AvatarUrl, UserId = p.UserId },
                Caption = p.Caption,
                CreatedAt = p.CreatedAt,
                Medias = p.Medias.Select(m => new PostMediaViewModel() { Url = m.Url, MediaType = m.MediaType }).ToList(),
                LikeCount = p.LikeCount,
                CommentCount = p.CommentCount,
                IsLiked = p.Likes.Any(l => l.UserId == userId),
                Hashtags = string.Join(" ", p.PostHashtags.Select(ph => ph.Hashtag.HashtagName))
            }).ToList();
        }
        public async Task<List<PostViewModel>> SearchPosts(string searchinfo, int userId) {
            var posts = await _postRepository.SearchPost(searchinfo);
            if(posts == null ) return new List<PostViewModel>();
            return posts.Select(p => new PostViewModel {
                PostId = p.PostId,
                Owner = new UserSummaryViewModel() { UserName = p.User?.UserName, FullName = p.User?.FullName, AvatarUrl = p.User?.AvatarUrl, UserId = p.UserId },
                Caption = p.Caption,
                CreatedAt = p.CreatedAt,
                Medias = p.Medias.Select(m => new PostMediaViewModel() { Url = m.Url, MediaType = m.MediaType }).ToList(),
                LikeCount = p.LikeCount,
                CommentCount = p.CommentCount,
                IsLiked = p.Likes.Any(l => l.UserId == userId),
                Hashtags =  string.Join(" ", p.PostHashtags.Select(ph => ph.Hashtag.HashtagName))
            }).ToList();
        }

        public async Task<List<MemoryViewModel>> GetMemoriesAsync(int userId) {
            var posts = await _postRepository.GetMemoriesAsync(userId);
            var currentYear = DateTime.UtcNow.Year;

            return posts.Select(p => new MemoryViewModel {
                PostId = p.PostId,
                Owner = new UserSummaryViewModel {
                    UserId = p.User.Id,
                    UserName = p.User.UserName,
                    FullName = p.User.FullName,
                    AvatarUrl = p.User.AvatarUrl
                },
                Caption = p.Caption,
                CreatedAt = p.CreatedAt,
                LikeCount = p.LikeCount,
                CommentCount = p.CommentCount,
                IsLiked = p.Likes.Any(l => l.UserId == userId),
                Medias = p.Medias.Select(m => new PostMediaViewModel {
                    Url = m.Url,
                    MediaType = m.MediaType
                }).ToList(),

                YearsAgo = currentYear - p.CreatedAt.Year
            }).ToList();
        }

        

        private int CalculateFeedScore(FeedItemViewModel item, int userId, Random random) {
            int score = 0;
            if (item.Type == "Share") {
                score += 70;
            }
            else {
                score += 80;
            }
            var originalPost = item.OriginalPost;

            if (originalPost != null) {
                if (!originalPost.IsLiked) {
                    score += 40;
                }
                else {
                    score -= 20;
                }

                if (originalPost.LikeCount > 50)
                    score += 30;
                else if (originalPost.LikeCount > 10)
                    score += 10;
            }
            var timeDiff = DateTime.UtcNow - item.DisplayTime;
            if (timeDiff.TotalHours < 2) {
                score += 40;
            }
            else if (timeDiff.TotalHours < 24) {
                score += 20;
            }
            else if (timeDiff.TotalDays < 2) {
                score += 10;
            }
            score += random.Next(0, 20);

            return score;
        }

        public async Task<IEnumerable<FeedItemViewModel>> GetNewsFeed(int userId, int page, int pageSize, int seed) {
            var posts = await _postRepository.GetNewsFeedPosts(userId);
            var shares = await _shareRepository.GetFriendsShare(userId);
            var suggested = await _postRepository.GetSuggestedPosts(userId, 100);

            posts.AddRange(suggested);
            Console.WriteLine("=======================================================================================================================================================================================");
            Console.WriteLine(shares.Count());

            var postItems = posts.Select(p => new FeedItemViewModel {
                Type = "Post",
                ItemId = p.PostId,
                DisplayTime = p.CreatedAt,
                Author = new UserSummaryViewModel { UserName = p.User?.UserName, FullName = p.User?.FullName, AvatarUrl = p.User?.AvatarUrl, UserId = p.UserId },
                OriginalPost = new PostViewModel {
                    PostId = p.PostId,
                    Owner = new UserSummaryViewModel() { UserName = p.User?.UserName, FullName = p.User?.FullName, AvatarUrl = p.User?.AvatarUrl, UserId = p.UserId },
                    Caption = p.Caption,
                    CreatedAt = p.CreatedAt,
                    Medias = p.Medias.Select(m => new PostMediaViewModel() { Url = m.Url, MediaType = m.MediaType }).ToList(),
                    LikeCount = p.LikeCount,
                    CommentCount = p.CommentCount,
                    IsLiked = p.Likes.Any(l => l.UserId == userId),
                    Hashtags = string.Join(" ", p.PostHashtags.Select(ph => ph.Hashtag.HashtagName))
                },
                ShareCaption = null
            });

            var shareItems = shares.Select(s => new FeedItemViewModel {
                Type = "Share",
                ItemId = s.Id,
                DisplayTime = s.SharedAt,
                Author = new UserSummaryViewModel { UserName = s.User?.UserName, FullName = s.User?.FullName, AvatarUrl = s.User?.AvatarUrl, UserId = s.UserId },
                OriginalPost = new PostViewModel {
                    PostId = s.PostId,
                    Owner = new UserSummaryViewModel() { UserName = s.Post.User?.UserName, FullName = s.Post.User?.FullName, AvatarUrl = s.Post.User?.AvatarUrl, UserId = s.Post.UserId },
                    Caption = s.Post.Caption,
                    CreatedAt = s.Post.CreatedAt,
                    Medias = s.Post.Medias.Select(m => new PostMediaViewModel() { Url = m.Url, MediaType = m.MediaType }).ToList(),
                    LikeCount = s.Post.LikeCount,
                    CommentCount = s.Post.CommentCount,
                    IsLiked = s.Post.Likes.Any(l => l.UserId == userId),
                    Hashtags = string.Join(" ", s.Post.PostHashtags.Select(ph => ph.Hashtag.HashtagName))
                },
                ShareCaption = s.Caption
            });
            var allFeed = postItems.Concat(shareItems).ToList();
            var random = new Random(seed);

            var sortedFeed = allFeed
                .Select(item => new {
                    Item = item,
                    Score = CalculateFeedScore(item, userId, random)
                })
                .OrderByDescending(x => x.Score)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => x.Item)
                .ToList();

            return sortedFeed;
        }
    }
}