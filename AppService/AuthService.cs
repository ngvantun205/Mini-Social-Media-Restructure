namespace Mini_Social_Media.AppService {
    public class AuthService : IAuthService {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;

        public AuthService(IUserRepository userRepository, ITokenService tokenService) {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }
        public async Task<RegisterDto> RegisterAsync(RegisterInputModel model) {
            if (await _userRepository.UserExist(model.UserName, model.Email)) {
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
            if (user == null) {
                return new LoginDto { ErrorMessage = "User not found." };
            }
            if (!BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash)) {
                return new LoginDto { ErrorMessage = "Incorrect Password." };
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
