using Microsoft.AspNetCore.SignalR;
using Mini_Social_Media.Models.DomainModel;

namespace Mini_Social_Media.AppService {
    public class PostService : IPostService {
        private readonly IPostRepository _postRepository;
        private readonly IUploadService _uploadService;
        private readonly IHashtagRepository _hashtagRepository;
        private readonly IPostMediaRepository _postMediaRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly IShareRepository _shareRepository;
        private readonly IAdRepository _adRepository;
        private readonly IGeminiService _geminiService;
        private readonly INotificationsRepository _notificationsRepository;
        private readonly IHubContext<NotificationsHub> _hubContext;

        public PostService(IPostRepository postRepository, IUploadService uploadService, IHashtagRepository hashtagRepository, IPostMediaRepository postMediaRepository, ICommentRepository commentRepository, IShareRepository shareRepository, IAdRepository adRepository, IGeminiService geminiService, INotificationsRepository notificationsRepository, IHubContext<NotificationsHub> hubContext) {
            _postRepository = postRepository;
            _uploadService = uploadService;
            _hashtagRepository = hashtagRepository;
            _postMediaRepository = postMediaRepository;
            _commentRepository = commentRepository;
            _shareRepository = shareRepository;
            _adRepository = adRepository;
            _geminiService = geminiService;
            _notificationsRepository = notificationsRepository;
            _hubContext = hubContext;
        }

        public async Task<PostViewModel> CreatePost(PostInputModel model, int userId) {
            var check = await _geminiService.CheckPost(model.Caption);
            if (check == false) {
                var noti = new Notifications() {
                    ActorId = userId,
                    ReceiverId = userId,
                    EntityId = userId,
                    Type = "Violation",
                    Content = "Your post you want to upload violated out terms of services.",
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow,
                };
                await _notificationsRepository.AddAsync(noti);

                await _hubContext.Clients.User(userId.ToString())
                .SendAsync("ReceiveNotification", new {
                    content = noti.Content,
                    type = noti.Type,
                    postId = noti.EntityId,
                    message = $"Some one has just liked post your post."
                });
                return null;
            }
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
            // 1. Lấy Posts và Shares (Logic cũ)
            var posts = await _postRepository.GetNewsFeedPosts(userId);
            var shares = await _shareRepository.GetFriendsShare(userId);
            var suggested = await _postRepository.GetSuggestedPosts(userId, 100);

            posts.AddRange(suggested);

            var runningAds = await _adRepository.GetAdsByStatusAsync(AdStatus.Running);
            var validAds = runningAds.Where(a => a.Type == AdType.SponsoredPost).ToList();

            var postItems = posts.Select(p => new FeedItemViewModel {
                Type = "Post",
                ItemId = p.PostId,
                DisplayTime = p.CreatedAt,
                Author = new UserSummaryViewModel {
                    UserName = p.User?.UserName,
                    FullName = p.User?.FullName,
                    AvatarUrl = p.User?.AvatarUrl,
                    UserId = p.UserId
                },
                OriginalPost = new PostViewModel {
                    PostId = p.PostId,
                    Caption = p.Caption,
                    Medias = p.Medias.Select(m => new PostMediaViewModel { Url = m.Url, MediaType = m.MediaType }).ToList(),
                    LikeCount = p.LikeCount,
                    CommentCount = p.CommentCount,
                    IsLiked = p.Likes.Any(l => l.UserId == userId),
                    Owner = new UserSummaryViewModel { UserName = p.User?.UserName, AvatarUrl = p.User?.AvatarUrl, UserId = p.UserId }
                }
            });

            // 4. Map Share sang ViewModel
            var shareItems = shares.Select(s => new FeedItemViewModel {
                Type = "Share",
                ItemId = s.Id,
                DisplayTime = s.SharedAt,
                Author = new UserSummaryViewModel {
                    UserName = s.User?.UserName,
                    FullName = s.User?.FullName,
                    AvatarUrl = s.User?.AvatarUrl,
                    UserId = s.UserId
                },
                ShareCaption = s.Caption,
                OriginalPost = new PostViewModel {
                    PostId = s.PostId,
                    Caption = s.Post.Caption,
                    Medias = s.Post.Medias.Select(m => new PostMediaViewModel { Url = m.Url, MediaType = m.MediaType }).ToList(),
                    LikeCount = s.Post.LikeCount,
                    CommentCount = s.Post.CommentCount,
                    IsLiked = s.Post.Likes.Any(l => l.UserId == userId),
                    Owner = new UserSummaryViewModel { UserName = s.Post.User?.UserName, AvatarUrl = s.Post.User?.AvatarUrl, UserId = s.Post.UserId }
                }
            });

            // 5. Gộp và Tính điểm (Score)
            var allContent = postItems.Concat(shareItems).ToList();
            var random = new Random(seed);

            // Sort bài viết trước
            var sortedFeed = allContent
                .Select(item => {
                    item.Score = CalculateFeedScore(item, userId, random); // Hàm tính điểm của bạn
                    return item;
                })
                .OrderByDescending(x => x.Score)
                .ToList();

            // 6. TRỘN QUẢNG CÁO (Logic chèn mỗi 5 bài 1 quảng cáo)
            var finalFeed = new List<FeedItemViewModel>();
            int adFrequency = 5;
            int adIndex = 0;

            for (int i = 0; i < sortedFeed.Count; i++) {
                finalFeed.Add(sortedFeed[i]);

                // Kiểm tra điều kiện chèn (sau bài thứ 5, 10, 15... và phải có quảng cáo để chèn)
                if ((i + 1) % adFrequency == 0 && validAds.Any()) {

                    // Lấy quảng cáo xoay vòng (Round Robin)
                    var adEntity = validAds[adIndex % validAds.Count];

                    // Map sang AdViewModel
                    var adViewModel = new AdViewModel {
                        Id = adEntity.Id,
                        Title = adEntity.Title,
                        Content = adEntity.Content,
                        ImageUrl = adEntity.ImageUrl,
                        TargetUrl = adEntity.TargetUrl,
                        CtaText = adEntity.CtaText,
                        Brand = new UserSummaryViewModel {
                            UserId = adEntity.UserId,
                            UserName = adEntity.User.UserName,
                            AvatarUrl = adEntity.User.AvatarUrl,
                            FullName = adEntity.User.FullName
                        },
                        Type = adEntity.Type,
                        Status = adEntity.Status
                    };

                    // Tạo FeedItemViewModel loại "Ad"
                    var adFeedItem = new FeedItemViewModel {
                        Type = "Ad",
                        ItemId = adEntity.Id,
                        DisplayTime = DateTime.UtcNow,
                        Author = adViewModel.Brand,   
                        Advertisement = adViewModel,  
                        OriginalPost = null          
                    };

                    finalFeed.Add(adFeedItem);
                    adIndex++;
                }
            }

            return finalFeed.Skip((page - 1) * pageSize).Take(pageSize);
        }
    }
}