namespace Mini_Social_Media.AppService {
    public class AuthService : IAuthService {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;

        public AuthService(IUserRepository userRepository) {
            _userRepository = userRepository;
        }
        public async Task<RegisterDto> RegisterAsync(RegisterInputModel model) {
            if(await _userRepository.UserExist(model.UserName, model.Email)) {
                throw new Exception("User already exists.");
            }
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);
            var user = new User {
                UserName = model.UserName,
                Email = model.Email,
                PasswordHash = hashedPassword,
                CreatedAt = DateTime.UtcNow,
                UpdateAt = DateTime.UtcNow
            };
            await _userRepository.AddAsync(user);

            return new RegisterDto {
                UserId = user.UserId,
                UserName = user.UserName,
                Email = user.Email
            };
        }
        public async Task<LoginDto> LoginAsync(LoginInputModel model) {
            var user = await _userRepository.GetByUserNameOrEmailAsync(model.UserNameOrEmail);
            if(user == null ){
                throw new Exception("Username or email does not exist.");
            }
            if(!BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash)) {
                throw new Exception("Incorrect password.");
            }
            string token = _tokenService.GenerateToken(user);
            return new LoginDto {
                UserId = user.UserId,
                UserName = user.UserName,
                Token = token
            };

        }
        public Task LogoutAsync() {
            return Task.CompletedTask;
        }
        
    }
}
