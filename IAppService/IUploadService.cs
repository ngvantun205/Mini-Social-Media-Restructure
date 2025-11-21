namespace Mini_Social_Media.IAppService {
    public interface IUploadService {
        Task<string> UploadAsync(IFormFile file);
    }
}
