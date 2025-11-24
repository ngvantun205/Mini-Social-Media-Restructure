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
                UserId = createdComment.UserId,
                Content = createdComment.Content,
                CreatedAt = createdComment.CreatedAt,
                UserAvatarUrl = createdComment.User?.AvatarUrl,
                UserName = createdComment.User?.UserName,
                FullName = createdComment.User?.FullName
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
                UserId = comment.UserId,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt
            };
        }
        public async Task<IEnumerable<CommentDto>> GetCommentsByPostIdAsync(int postId) {
            var comments = await _commentRepository.GetCommentsByPostIdAsync(postId);
            return comments.Select(c => new CommentDto {
                CommentId = c.CommentId,
                UserId = c.UserId,
                Content = c.Content,
                CreatedAt = c.CreatedAt,
                UserAvatarUrl = c.User?.AvatarUrl,
                UserName = c.User?.UserName,
                FullName = c.User?.FullName
            });
        }
        public async Task<IEnumerable<ReplyCommentDto>> GetRepliesByCommentIdAsync(int commentId) {
            var comment = await _commentRepository.GetByIdAsync(commentId);
            if (comment == null) {
                return Enumerable.Empty<ReplyCommentDto>();
            }
            var replies = comment.Replies ?? Enumerable.Empty<Comment>();
            return replies.Select(r => new ReplyCommentDto {
                CommentId = r.CommentId,
                UserId = r.UserId,
                Content = r.Content,
                CreatedAt = r.CreatedAt,
                UserName = r.User?.UserName,
                UserAvatarUrl = r.User?.AvatarUrl
            });
        }
        public async Task<ReplyCommentDto?> AddReplyAsync(ReplyCommentInputModel model, int userId) {
            var reply = new Comment {
                UserId = userId,
                Content = model.Content,
                CreatedAt = DateTime.UtcNow,
                PostId = model.PostId,
                ParentCommentId = model.ParentCommentId
            };
            var result = await _postRepository.AddCommentAsync(model.PostId);
            if (!result)
                return null!;
            await _commentRepository.AddAsync(reply);
            var createdreply = await _commentRepository.GetByIdAsync(reply.CommentId);
            return new ReplyCommentDto {
                CommentId = createdreply.CommentId,
                UserId = createdreply.UserId,
                Content = createdreply.Content,
                CreatedAt = createdreply.CreatedAt,
                ParentCommentId = createdreply.ParentCommentId,
                UserName = createdreply.User?.UserName,
                UserAvatarUrl = createdreply.User?.AvatarUrl,
                FullName = createdreply.User?.FullName
            };
        }
    }
}
