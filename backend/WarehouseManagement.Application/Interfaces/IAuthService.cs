using WarehouseManagement.Application.DTOs.Auth;

namespace WarehouseManagement.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginRequest);
    Task<bool> RegisterAsync(string username, string password, string role);
}
