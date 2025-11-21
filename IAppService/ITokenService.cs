namespace Mini_Social_Media.IAppService {
    public interface ITokenService {
        string GenerateToken(User user);
    }
}
