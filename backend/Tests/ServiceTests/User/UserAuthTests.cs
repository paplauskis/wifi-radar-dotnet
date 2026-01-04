using API.Data.Repositories.Interfaces;
using API.Domain.Dto;
using API.Domain.Exceptions;
using API.Helpers;
using API.Services.Interfaces.Auth;
using API.Services.Users;
using Moq;
using Xunit;

namespace Tests.ServiceTests.User;

public class UserAuthTests
{
    [Fact]
    public async Task HandleUserLogin_WithValidUser_ReturnsLoginResponse()
    {
        var request = new UserLoginRequestDto { Username = "User1", Password = "Password1" };
        var expected = new UserLoginResponseDto { Id = "id", Username = "User1", AccessToken = "token", ExpiresIn = 3600 };

        var jwt = new Mock<IAuthenticatable>();
        var repo = new Mock<IUserRepository>();
        var passwordHelper = new Mock<IPasswordHelper>();
        jwt.Setup(j => j.AuthenticateUser(request)).ReturnsAsync(expected);

        var service = new UserAuthService(jwt.Object, repo.Object, passwordHelper.Object);
        var result = await service.HandleUserLogin(request);

        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task HandleUserLogin_WithInvalidUser_ThrowsUnauthorizedAccessException()
    {
        var request = new UserLoginRequestDto { Username = "User1", Password = "Password1" };

        var jwt = new Mock<IAuthenticatable>();
        var repo = new Mock<IUserRepository>();
        var passwordHelper = new Mock<IPasswordHelper>();
        jwt.Setup(j => j.AuthenticateUser(request)).ReturnsAsync((UserLoginResponseDto?)null);

        var service = new UserAuthService(jwt.Object, repo.Object, passwordHelper.Object);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.HandleUserLogin(request));
    }

    [Theory]
    [InlineData(null, "Password1")]
    [InlineData("", "Password1")]
    [InlineData("User1", null)]
    [InlineData("User1", "")]
    public async Task HandleUserRegistration_WithMissingCredentials_ThrowsArgumentNullException(
        string? username,
        string? password)
    {
        var request = new UserLoginRequestDto { Username = username, Password = password };

        var jwt = new Mock<IAuthenticatable>();
        var repo = new Mock<IUserRepository>();
        var passwordHelper = new Mock<IPasswordHelper>();

        var service = new UserAuthService(jwt.Object, repo.Object, passwordHelper.Object);

        await Assert.ThrowsAsync<ArgumentNullException>(() => service.HandleUserRegistration(request));
    }

    [Theory]
    [InlineData("password")]
    [InlineData("Pass1")]
    [InlineData("PASSWORD1")]
    public async Task HandleUserRegistration_WithInvalidPassword_ThrowsArgumentException(string password)
    {
        var request = new UserLoginRequestDto { Username = "User1", Password = password };
        Assert.False(UserValidator.IsPasswordValid(password));

        var jwt = new Mock<IAuthenticatable>();
        var repo = new Mock<IUserRepository>();
        var passwordHelper = new Mock<IPasswordHelper>();

        var service = new UserAuthService(jwt.Object, repo.Object, passwordHelper.Object);

        await Assert.ThrowsAsync<ArgumentException>(() => service.HandleUserRegistration(request));
    }

    [Fact]
    public async Task HandleUserRegistration_WhenUserAlreadyExists_ThrowsUserAlreadyExistsException()
    {
        var request = new UserLoginRequestDto { Username = "User1", Password = "Password1" };
        var existing = new API.Domain.Models.User { Id = "id", Username = "User1", Password = "hashed" };

        var jwt = new Mock<IAuthenticatable>();
        var repo = new Mock<IUserRepository>();
        var passwordHelper = new Mock<IPasswordHelper>();
        repo.Setup(r => r.GetByUsernameAsync(request.Username)).ReturnsAsync(existing);

        var service = new UserAuthService(jwt.Object, repo.Object, passwordHelper.Object);

        await Assert.ThrowsAsync<UserAlreadyExistsException>(() => service.HandleUserRegistration(request));
    }

    [Fact]
    public async Task HandleUserRegistration_WithValidData_AddsUserAndReturnsLogin()
    {
        var request = new UserLoginRequestDto { Username = "User1", Password = "Password1" };
        var expected = new UserLoginResponseDto { Id = "id", Username = "User1", AccessToken = "token", ExpiresIn = 3600 };

        var jwt = new Mock<IAuthenticatable>();
        var repo = new Mock<IUserRepository>();
        var passwordHelper = new Mock<IPasswordHelper>();

        repo.Setup(r => r.GetByUsernameAsync(request.Username)).ReturnsAsync((API.Domain.Models.User?)null);
        passwordHelper.Setup(p => p.HashPassword(It.IsAny<API.Domain.Models.User>(), request.Password))
            .Returns("hashed");
        repo.Setup(r => r.AddAsync(It.IsAny<API.Domain.Models.User>()))
            .ReturnsAsync((API.Domain.Models.User u) => u);
        jwt.Setup(j => j.AuthenticateUser(request)).ReturnsAsync(expected);

        var service = new UserAuthService(jwt.Object, repo.Object, passwordHelper.Object);
        var result = await service.HandleUserRegistration(request);

        Assert.Equal(expected, result);
        repo.Verify(r => r.AddAsync(It.Is<API.Domain.Models.User>(
            u => u.Username == request.Username && u.Password == "hashed")), Times.Once);
    }
}
