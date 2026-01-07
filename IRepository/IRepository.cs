namespace Mini_Social_Media.IRepository {
    public interface IRepository<T> where T : class {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
    }
    public interface IUserRepository : IRepository<User> {  //User
        Task<bool> UserExist(string userName, string email);
        Task<User?> GetByUserNameOrEmailAsync(string userNameOrEmail);
        Task UpdatePrivacy(bool isPrivate, int userId);
        Task<bool> AddFollowAsync(int followeeId, int followerId);
        Task<bool> DeleteFollowAsync(int followeeId, int followerId);
        Task<IEnumerable<User>> SearchUsersAsync(string searchinfo);
    }
    public interface IPostRepository : IRepository<Post> {   //Post
        Task<IEnumerable<Post>> GetPostsPagedAsync(int page, int pageSize);
        Task LikePostAsync(int postId);
        Task UnLikePostAsync(int postId);
        Task<bool> AddCommentAsync(int postId);
        Task<bool> RemoveCommentAsync(int postId);
        Task<IEnumerable<Post>> SearchPost(string searchinfo);
        Task<List<Post>> GetNewsFeedPosts(int userId);
        Task<List<Post>> GetSuggestedPosts(int userId, int limit);
        Task<IEnumerable<Post>> GetMemoriesAsync(int userId);
        Task<string> GetOwnerUsername(int postId);
    }
    public interface ICommentRepository : IRepository<Comment> {  //Comment
        Task<IEnumerable<Comment>> GetCommentsByPostIdAsync(int postId);
        Task<bool> AddReplyAsync(int commentId);    
        Task<bool> RemoveReplyAsync(int commentId);
        Task<IEnumerable<Comment>> GetRepliesByCommentIdAsync(int commentId);
        Task<IEnumerable<Comment>> GetUserHistoryComment(int userId);
    }
    public interface ILikeRepository : IRepository<Like> {  //Like
        Task DeleteByPostIdAndUserIdAsync(int postId, int userId);
        Task<bool> IsLikedByCurrentUser(int postId, int userId); 
        Task<IEnumerable<Like>> GetUserHistoryLike(int userId);
    }
    public interface IFollowRepository : IRepository<Follow> {   //Follow
        Task<Follow?> GetFollowByUserAsync(int followeeId, int followerId);
        Task<IEnumerable<Follow>> GetFolloweeAsync(int userId);
        Task<IEnumerable<Follow>> GetFollowerAsync(int userId);
    }
    public interface IHashtagRepository : IRepository<Hashtag> {   //Hashtag
        Task<bool> IsExist(string hashtagName);
        Task<Hashtag?> GetByNameAsync(string hashtagName);
        Task<IEnumerable<Hashtag>> GetTopHashtag();
    }
    public interface IPostMediaRepository : IRepository<PostMedia> {
        Task<PostMedia?> GetByUrlAsync(string url);
        Task RemoveRangeAsync(IEnumerable<PostMedia> postMedias);
    }
    public interface INotificationsRepository : IRepository<Notifications> {
        Task<IEnumerable<Notifications>> GetByReceiverIdAsync(int receiverId);
        Task MarkAsReadAsync(int notificationId);
        Task MarkAllAsReadAsync(int userId);
        Task<Notifications?> GetNotification(int actorId, int receiverId, string type, int entityId);
    }
    public interface IMessageRepository {
        Task AddMessageAsync(Messages message);
        Task<Conversations?> GetConversationAsync(int userId1, int userId2);
        Task CreateConversationAsync(Conversations conversation);
        Task UpdateConversationAsync(Conversations conversation);
        Task<IEnumerable<Messages>> GetMessagesAsync(int conversationId);
        Task<IEnumerable<Conversations>> GetUserConversationsAsync(int userId);
        Task MarkAsRead(int userId, int partnerId);
    }
    public interface IReportRepository : IRepository<Report> {
        Task<IEnumerable<Report>> FilterReportByType(string type);
        Task<IEnumerable<Report>> FilterReportsByStatus(ReportStatus status);
    }
    public interface IStoryRepository : IRepository<Story> {
        Task<IEnumerable<Story>> GetCurrentStories(int userId);
        Task<IEnumerable<Story>> GetFriendsStories(int currentUserId);

    }
    public interface IStoryArchiveRepository {
        Task<IEnumerable<StoryArchive>> GetUserStoryArchive(int userId);
        Task DeleteAsync(int id);
        Task AddAsync(StoryArchive entity);
    }
    public interface IShareRepository : IRepository<Share> {
        Task<IEnumerable<Share>> GetFriendsShare(int userId);
    }
    public interface IAdRepository : IRepository<Advertisement> {
        Task<bool> ApproveAd(int adId);
        Task<bool> DeclineAd(int adId);
        Task<bool> DisableAd(int adId);
        Task<bool> CancelRequestAd(int adId);
        Task<IEnumerable<Advertisement>> GetUserAd(int userId);
        Task<IEnumerable<Advertisement>> GetAdsByStatusAsync(AdStatus status);
        Task<Advertisement> GetRandomBanner();
    }
}
