namespace Mini_Social_Media.IAppService {
    public interface IAuthService {
        Task<LoginDto> LoginAsync(LoginInputModel model);
        Task LogoutAsync();
        Task<RegisterDto> RegisterAsync(RegisterInputModel model);
    }
}
