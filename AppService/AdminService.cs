
namespace Mini_Social_Media.AppService {
    public class AdminService : IAdminService {
        private readonly IUserRepository _userRepository;
        private readonly IPostRepository _postRepository;
        private readonly IReportRepository _reportRepository;
        public AdminService(IUserRepository userRepository, IPostRepository postRepository, IReportRepository reportRepository) {
            _userRepository = userRepository;
            _postRepository = postRepository;
            _reportRepository = reportRepository;
        }
        public async Task<IEnumerable<UserSummaryViewModel>> GetAllUser() {
            var users = await _userRepository.GetAllAsync();
            return users.Select(x => new UserSummaryViewModel() {
                UserId = x.Id,
                FullName = x.FullName,
                UserName = x.UserName,
                AvatarUrl = x.AvatarUrl,
            });
        }
        public async Task<IEnumerable<UserSummaryViewModel>> SearchUser(string searchinfo) {
            var users = await _userRepository.SearchUsersAsync(searchinfo);
            return users.Select(x => new UserSummaryViewModel() {
                UserId = x.Id,
                FullName = x.FullName,
                UserName = x.UserName,
                AvatarUrl = x.AvatarUrl,
            });
        }
        public async Task DeleteUser(int userId) {
            await _userRepository.DeleteAsync(userId);
        }
        public async Task<IEnumerable<PostSummaryViewModel>> GetAllPosts() {
            var posts = await _postRepository.GetAllAsync();
            if (posts == null)
                return new List<PostSummaryViewModel>();
            return posts.Select(p => new PostSummaryViewModel() {
                PostId = p.PostId,
                UserId = p.UserId,
                CommentCount = p.CommentCount,
                LikeCount = p.LikeCount,
                MediaUrl = p.Medias != null && p.Medias.Count > 0 ? p.Medias[0].Url : null
            });
        }
        public async Task<IEnumerable<PostSummaryViewModel>> SearchPosts(string searchinfo) {
            var posts = await _postRepository.SearchPost(searchinfo);
            if (posts == null)
                return new List<PostSummaryViewModel>();
            return posts.Select(p => new PostSummaryViewModel() {
                PostId = p.PostId,
                UserId = p.UserId,
                CommentCount = p.CommentCount,
                LikeCount = p.LikeCount,
                MediaUrl = p.Medias != null && p.Medias.Count > 0 ? p.Medias[0].Url : null
            });
        }
        public async Task DeletePost(int postId) {
            await _postRepository.DeleteAsync(postId);
        }
        public async Task<IEnumerable<ReportViewModel>> GetAllReports() {
            var reports = await _reportRepository.GetAllAsync();
            if (reports == null)
                return new List<ReportViewModel>();
            return reports.Select(p => new ReportViewModel() {
                ReportId = p.ReportId,
                ReporterId = p.ReporterId,
                Type = p.Type,
                EntityId = p.EntityId,
                CreatedAt = p.CreatedAt,
                Content = p.Content,
                IsExecuted = p.IsExecuted,
            });
        }
        public async Task<ReportViewModel> ViewReportDetail(int reportId) {
            var report = await _reportRepository.GetByIdAsync(reportId);
            return new ReportViewModel() {
                ReportId = report.ReportId,
                ReporterId = report.ReporterId,
                Type = report.Type,
                EntityId = report.EntityId,
                CreatedAt = report.CreatedAt,
                Content = report.Content,
                IsExecuted = report.IsExecuted,
            };
        }
        public async Task ExecuteReport(int reportId) {
            var report = await _reportRepository.GetByIdAsync(reportId);
            if (report == null)
                return;
            report.IsExecuted = true;
            await _reportRepository.UpdateAsync(report);
        }
        public async Task<IEnumerable<ReportViewModel>> FilterReportsByStatus(ReportStatus status) {
            var reports = await _reportRepository.FilterReportsByStatus(status);
            if (reports == null)
                return new List<ReportViewModel>();
            return reports.Select(p => new ReportViewModel() {
                ReportId = p.ReportId,
                ReporterId = p.ReporterId,
                Type = p.Type,
                EntityId = p.EntityId,
                CreatedAt = p.CreatedAt,
                Content = p.Content,
                IsExecuted = p.IsExecuted,
            });
        }
    }
}
