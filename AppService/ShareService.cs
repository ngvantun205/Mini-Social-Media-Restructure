namespace Mini_Social_Media.AppService {
    public class ShareService : IShareService {
        private readonly IShareRepository _shareRepository;
        public ShareService(IShareRepository shareRepository) {
            _shareRepository = shareRepository;
        }
        public async Task AddShare(ShareInputModel shareInputModel, int userId) {
            var share = new Share() {
                PostId = shareInputModel.PostId,
                Caption = shareInputModel.Caption,
                UserId = userId,
                SharedAt = DateTime.UtcNow,
            };
            await _shareRepository.AddAsync(share);
        }
        public async Task DeleteShare(int shareId) {
            await _shareRepository.DeleteAsync(shareId);
        }
        public async Task EditShare(EditShareInputModel inputModel, int userId) {
            var share = await _shareRepository.GetByIdAsync(inputModel.ShareId);
            if (share == null) return;
            if (share.UserId != userId)
                return;
            share.Caption = inputModel.Caption;
            await _shareRepository.UpdateAsync(share);
        }
    }
}
