using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Application.DTOs.Auth;
using WarehouseManagement.Application.Interfaces;

namespace WarehouseManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto loginRequest)
    {
        var result = await _authService.LoginAsync(loginRequest);
        
        if (result == null)
        {
            return Unauthorized(new { message = "Invalid username or password" });
        }

        return Ok(result);
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult> Register([FromBody] LoginRequestDto registerRequest)
    {
        var result = await _authService.RegisterAsync(registerRequest.Username, registerRequest.Password, "User");
        
        if (!result)
        {
            return BadRequest(new { message = "Username already exists" });
        }

        return Ok(new { message = "User registered successfully" });
    }
}
