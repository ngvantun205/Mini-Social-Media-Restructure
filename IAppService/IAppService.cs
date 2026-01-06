using Microsoft.AspNetCore.Identity;

namespace Mini_Social_Media.IAppService {
    public interface IPostService {
        Task<PostViewModel> CreatePost(PostInputModel model, int userId);
        Task<PostViewModel?> GetByIdAsync(int postId, int userId);
        Task<PostViewModel?> EditPostAsync(EditPostInputModel model, int userId);
        Task<bool> DeletePostAsync(int postId, int userId);
        Task<IEnumerable<PostViewModel>> GetPostsPagedAsync(int pageNumber, int pageSize, int userId);
        Task<List<PostViewModel>> SearchPosts(string searchinfo, int userId);
        Task<List<MemoryViewModel>> GetMemoriesAsync(int userId);
        Task<IEnumerable<FeedItemViewModel>> GetNewsFeed(int userId, int page, int pageSize, int seed);
    }
    public interface IUploadService {
        Task<string> UploadAsync(IFormFile file);
    }
    public interface IEmailService {
        Task SendEmailAsync(string toEmail, string subject, string message);
    }
    public interface ILikeService {
        Task<LikeDto> ToggleLikeAsync(LikeInputModel inputModel, int userId);
        Task<IEnumerable<LikeViewModel>> GetUserHistoryLike(int userId);
    }
    public interface ICommentService {
        Task<CommentViewModel>? AddCommentAsync(CommentInputModel model, int userId);
        Task<bool> DeleteCommentAsync(int commentId, int userId);
        Task<CommentViewModel?> EditCommentAsync(EditCommentInputModel model, int userId);
        Task<IEnumerable<CommentViewModel>> GetCommentsByPostIdAsync(int postId);
        Task<IEnumerable<CommentViewModel>> GetRepliesByCommentIdAsync(int commentId);
        Task<CommentViewModel?> AddReplyAsync(ReplyCommentInputModel model, int userId);
        Task<IEnumerable<CommentViewModel>> GetUserHistoryComment(int userId);
    }
    public interface IUserService {
        Task<UserProfileViewModel?> GetUserProfileAsync(int userId, int requesterId);
        Task<MyProfileViewModel?> GetMyProfileAsync(int userId);
        Task<MyProfileViewModel> UpdateUserAvatar(IFormFile formFile, int userId);
        Task<EditProfileViewModel> GetEditProfile(int userId);
        Task<EditProfileViewModel> Edit(EditProfileInputModel model, int userId);
        Task<IdentityResult> ChangePassword(ChangePasswordInputModel changeInput, int userId);
        Task ChangeAccountPrivacy(bool isPrivate, int userId);
        Task<IEnumerable<UserSummaryViewModel>> SearchUsers(string searchinfo);
    }
    public interface INotificationsService {
        Task CreateNotification(int senderId, int receiverId, string type, int entityId, string message);
        Task<IEnumerable<NotificationsViewModel>> GetUserNotifications(int userId);
        Task MarkAllAsReadAsync(int userId);
    }
    public interface IMessageService {
        Task<MessageViewModel> SendMessageAsync(int senderId, int receiverId, string content);
        Task<IEnumerable<ConversationViewModel>> GetUserConversationsAsync(int userId);
        Task<IEnumerable<MessageViewModel>> GetMessageHistoryAsync(int currentUserId, int partnerId);
        Task<ConversationViewModel>? GetOrCreateConversationAsync(int userId, int receiverId);
        Task MarkConversationAsReadAsync(int userId, int partnerId);
        Task<MessageViewModel> SendImgOrVoiceAsync(int senderId, SendImgOrVoiceInputModel model);
    }
    public interface IHashtagService {
        Task<IEnumerable<Hashtag>> GetTopHashtag();
    }
    public interface IFollowService {
        Task<FollowDto> Follow(FollowInputModel inputModel, int followerId);
        Task<FollowDto> Unfollow(FollowInputModel inputModel, int followerId);
        Task<IEnumerable<FollowViewModel>> GetFollowers(int userId);
        Task<IEnumerable<FollowViewModel>> GetFollowee(int userId);
    }
    public interface IAdminService {
        Task<IEnumerable<UserSummaryViewModel>> GetAllUser();
        Task<IEnumerable<UserSummaryViewModel>> SearchUser(string searchinfo);
        Task DeleteUser(int userId);
        Task<IEnumerable<PostSummaryViewModel>> GetAllPosts();
        Task<IEnumerable<PostSummaryViewModel>> SearchPosts(string searchinfo);
        Task DeletePost(int postId);
        Task<IEnumerable<ReportViewModel>> GetAllReports();
        Task<ReportViewModel> ViewReportDetail(int reportId);
        Task ExecuteReport(int reportId);
        Task<IEnumerable<ReportViewModel>> FilterReportsByStatus(ReportStatus status);
    }
    public interface IReportService {
        Task<ReportViewModel> AddReport(ReportInputModel inputModel, int userId);
    }
    public interface IStoryService {
        Task<StoryViewModel> AddStory(StoryInputModel inputModel, int userId);
        Task DeleteStory(int storyId);
        Task<IEnumerable<UserStoryViewModel>> GetCurrentStories(int userId);
        Task<IEnumerable<StoryArchiveViewModel>> GetUserStoryArchives(int userId);
        Task DeleteArchive(int archiveId);
    }
    public interface IShareService {
        Task AddShare(ShareInputModel shareInputModel, int userId);
        Task DeleteShare(int shareId);
        Task EditShare(EditShareInputModel inputModel, int userId);
    }
    public interface IAdService {
        Task RequestAd(AdInputModel inputModel, int userId);
        Task<bool> DisableAd(int  adId, int userId );
        Task<bool> ApprovedAd(int adId, int userId);
        Task<bool> DeclineAd(int adId, int userId);
        Task<AdViewModel> GetById(int adId);
        Task<bool> UpdateAd(EditAdInputModel inputModel, int userId);
        Task<IEnumerable<AdViewModel>> GetUserAd(int userId);
        Task<bool> CancelRequest(int adId, int userId);
        Task<IEnumerable<AdViewModel>> GetAdsByStatusForAdmin(int userId, string statusStr);
    }
}
