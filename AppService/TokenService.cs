using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class TokenService : ITokenService {
    private readonly IConfiguration _config;

    public TokenService(IConfiguration config) {
        _config = config;

    }

    public string GenerateToken(User user) {
        var claims = new[]
        {
            new Claim("userId", user.UserId.ToString()),
            new Claim("userName", user.UserName)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"])
        );

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expire = DateTime.UtcNow.AddMinutes(
           int.Parse(_config["Jwt:ExpireMinutes"])
           );
        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: expire,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
