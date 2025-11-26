namespace Mini_Social_Media.AppService {
    public class CommentService : ICommentService {
        private ICommentRepository _commentRepository;
        private IPostRepository _postRepository;
        public CommentService(ICommentRepository commentRepository, IPostRepository postRepository) {
            _commentRepository = commentRepository;
            _postRepository = postRepository;
        }
        public async Task<CommentDto>? AddCommentAsync(CommentInputModel model, int userId) {
            var comment = new Comment {
                UserId = userId,
                Content = model.Content,
                CreatedAt = DateTime.UtcNow,
                PostId = model.PostId,
                ReplyCount = 0,
                ParentComment = null,
                ParentCommentId = null
            };
            await _commentRepository.AddAsync(comment);
            var result = await _postRepository.AddCommentAsync(model.PostId);
            if (!result)
                return null;
            var createdComment = await _commentRepository.GetByIdAsync(comment.CommentId);
            return new CommentDto {
                CommentId = createdComment.CommentId,
                Content = createdComment.Content,
                CreatedAt = createdComment.CreatedAt,
                Owner = new UserSummaryDto() {UserId = createdComment.User.Id, UserName = createdComment.User.UserName, FullName = createdComment.User.FullName, AvatarUrl = createdComment.User.AvatarUrl  }
            };
        }
        public async Task<bool> DeleteCommentAsync(int commentId, int userId) {
            var comment = await _commentRepository.GetByIdAsync(commentId);
            if (comment == null || comment.UserId != userId) {
                return false;
            }
            if (comment.ParentComment != null) {
                var resultReply = await _commentRepository.RemoveReplyAsync(int.Parse(comment.ParentCommentId.ToString()));
                if (!resultReply)
                    return false;
            }
            await _commentRepository.DeleteAsync(commentId);
            var result = await _postRepository.RemoveCommentAsync(comment.PostId);
            if (!result)
                return false;
            return true;
        }
        public async Task<CommentDto?> EditCommentAsync(EditCommentInputModel model, int userId) {
            var comment = await _commentRepository.GetByIdAsync(model.CommentId);
            if (comment == null || comment.UserId != userId) {
                return null;
            }
            comment.Content = model.Content;
            await _commentRepository.UpdateAsync(comment);
            return new CommentDto {
                CommentId = comment.CommentId,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                Owner = new UserSummaryDto() { UserId = comment.User.Id, UserName = comment.User.UserName, FullName = comment.User.FullName, AvatarUrl = comment.User.AvatarUrl }
            };
        }
        public async Task<IEnumerable<CommentDto>> GetCommentsByPostIdAsync(int postId) {
            var comments = await _commentRepository.GetCommentsByPostIdAsync(postId);
            return comments.Select(c => new CommentDto {
                CommentId = c.CommentId,
                Content = c.Content,
                CreatedAt = c.CreatedAt,
                ReplyCount = c.ReplyCount,
                Owner = new UserSummaryDto() { UserId = c.User.Id, UserName = c.User.UserName, FullName = c.User.FullName, AvatarUrl = c.User.AvatarUrl }
            });
        }
        public async Task<IEnumerable<CommentDto>> GetRepliesByCommentIdAsync(int commentId) {
            var comment = await _commentRepository.GetByIdAsync(commentId);
            if (comment == null) {
                return Enumerable.Empty<CommentDto>();
            }
            var replies = await _commentRepository.GetRepliesByCommentIdAsync(commentId);
            return replies.Select(r => new CommentDto {
                CommentId = r.CommentId,
                Content = r.Content,
                CreatedAt = r.CreatedAt,
                ReplyCount = r.ReplyCount,
                Owner = new UserSummaryDto() { UserId = r.User.Id, UserName = r.User.UserName, FullName = r.User.FullName, AvatarUrl = r.User.AvatarUrl }
            });
        }
        public async Task<CommentDto?> AddReplyAsync(ReplyCommentInputModel model, int userId) {
            var reply = new Comment {
                UserId = userId,
                Content = model.Content,
                CreatedAt = DateTime.UtcNow,
                PostId = model.PostId,
                ParentCommentId = model.ParentCommentId
            };
            var result = await _postRepository.AddCommentAsync(model.PostId);
            if (!result)
                return null;
            var resultReply = await _commentRepository.AddReplyAsync(model.ParentCommentId);
            if (!resultReply)
                return null;
            await _commentRepository.AddAsync(reply);
            var createdreply = await _commentRepository.GetByIdAsync(reply.CommentId);
            return new CommentDto {
                CommentId = createdreply.CommentId,
                Content = createdreply.Content,
                CreatedAt = createdreply.CreatedAt,
                ReplyCount = createdreply.ReplyCount,
                ParentCommentId = createdreply.ParentCommentId,
                Owner = new UserSummaryDto() { UserId = createdreply.User.Id, UserName = createdreply.User.UserName, FullName = createdreply.User.FullName, AvatarUrl = createdreply.User.AvatarUrl }
            };
        }
    }
}
