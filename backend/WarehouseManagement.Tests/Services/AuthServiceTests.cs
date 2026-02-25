using Microsoft.Extensions.Configuration;
using NSubstitute;
using NUnit.Framework;
using WarehouseManagement.Application.DTOs.Auth;
using WarehouseManagement.Application.Services;
using WarehouseManagement.Domain.Entities;
using WarehouseManagement.Infrastructure.Repositories.Interfaces;

namespace WarehouseManagement.Tests.Services;

[TestFixture]
public class AuthServiceTests
{
    private IUserRepository _userRepository = null!;
    private IConfiguration _configuration = null!;
    private AuthService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _configuration = Substitute.For<IConfiguration>();

        _configuration["Jwt:Key"].Returns("12345678901234567890123456789012");
        _configuration["Jwt:Issuer"].Returns("WarehouseManagementApi");
        _configuration["Jwt:Audience"].Returns("WarehouseManagementClient");

        _service = new AuthService(_userRepository, _configuration);
    }

    [Test]
    public async Task LoginAsync_WhenUserNotFound_ReturnsNull()
    {
        var request = new LoginRequestDto { Username = "missing", Password = "pass" };
        _userRepository.GetByUsernameAsync("missing").Returns((User?)null);

        var result = await _service.LoginAsync(request);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task LoginAsync_WhenPasswordInvalid_ReturnsNull()
    {
        var request = new LoginRequestDto { Username = "worker", Password = "wrong" };
        _userRepository.GetByUsernameAsync("worker").Returns(new User
        {
            Id = 2,
            Username = "worker",
            PasswordHash = "Worker123",
            Role = "Worker"
        });

        var result = await _service.LoginAsync(request);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task LoginAsync_WhenCredentialsValid_ReturnsTokenAndUserData()
    {
        var request = new LoginRequestDto { Username = "admin", Password = "Admin123" };
        _userRepository.GetByUsernameAsync("admin").Returns(new User
        {
            Id = 1,
            Username = "admin",
            PasswordHash = "Admin123",
            Role = "Admin"
        });

        var result = await _service.LoginAsync(request);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Username, Is.EqualTo("admin"));
        Assert.That(result.Role, Is.EqualTo("Admin"));
        Assert.That(result.Token, Is.Not.Empty);
        Assert.That(result.RefreshToken, Is.Not.Empty);
    }

    [Test]
    public async Task RegisterAsync_WhenUsernameExists_ReturnsFalse()
    {
        _userRepository.GetByUsernameAsync("existing").Returns(new User { Id = 3, Username = "existing" });

        var result = await _service.RegisterAsync("existing", "Password123", "Worker");

        Assert.That(result, Is.False);
        await _userRepository.DidNotReceive().AddAsync(Arg.Any<User>());
    }

    [Test]
    public async Task RegisterAsync_WhenUsernameUnique_CreatesUserAndReturnsTrue()
    {
        _userRepository.GetByUsernameAsync("new-user").Returns((User?)null);

        var result = await _service.RegisterAsync("new-user", "Password123", "Worker");

        Assert.That(result, Is.True);
        await _userRepository.Received(1).AddAsync(Arg.Is<User>(u =>
            u.Username == "new-user" &&
            u.Role == "Worker" &&
            !string.IsNullOrWhiteSpace(u.PasswordHash) &&
            u.PasswordHash != "Password123"));
    }
}
