using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using WarehouseManagement.Application.DTOs.Auth;
using WarehouseManagement.Application.Interfaces;
using WarehouseManagement.Domain.Entities;
using WarehouseManagement.Infrastructure.Repositories.Interfaces;

namespace WarehouseManagement.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly string _jwtKey;
    private readonly string _jwtIssuer;
    private readonly string _jwtAudience;

    public AuthService(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _jwtKey = configuration["Jwt:Key"] ?? "YourSecretKeyHere123456789012345678901234567890";
        _jwtIssuer = configuration["Jwt:Issuer"] ?? "WarehouseManagementApi";
        _jwtAudience = configuration["Jwt:Audience"] ?? "WarehouseManagementClient";
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginRequest)
    {
        var user = await _userRepository.GetByUsernameAsync(loginRequest.Username);

        if (user == null || !VerifyPassword(loginRequest.Password, user.PasswordHash))
        {
            return null;
        }

        if (IsLegacyPasswordHash(user.PasswordHash))
        {
            user.PasswordHash = HashPassword(loginRequest.Password);
            await _userRepository.UpdateAsync(user);
        }

        var token = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();

        return new LoginResponseDto
        {
            Token = token,
            RefreshToken = refreshToken,
            Username = user.Username,
            Role = user.Role
        };
    }

    public async Task<bool> RegisterAsync(string username, string password, string role)
    {
        var existingUser = await _userRepository.GetByUsernameAsync(username);
        if (existingUser != null)
        {
            return false;
        }

        var user = new User
        {
            Username = username,
            PasswordHash = HashPassword(password),
            Role = role,
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user);
        return true;
    }

    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var token = new JwtSecurityToken(
            issuer: _jwtIssuer,
            audience: _jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
        return Guid.NewGuid().ToString();
    }

    private static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    private static bool VerifyPassword(string password, string passwordHash)
    {
        if (IsLegacyPasswordHash(passwordHash))
        {
            return password == passwordHash;
        }

        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }

    private static bool IsLegacyPasswordHash(string passwordHash)
    {
        return !passwordHash.StartsWith("$2a$")
            && !passwordHash.StartsWith("$2b$")
            && !passwordHash.StartsWith("$2y$");
    }
}
