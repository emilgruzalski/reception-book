using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Contracts;
using Entities.ConfigurationModels;
using Entities.Exceptions;
using Entities.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using ReceptionBook.Tests.Helpers;
using Service;
using Shared.DataTransferObjects;

namespace ReceptionBook.Tests.Services;

public class AuthenticationServiceTests
{
    private const string Password = "Reception!2024pass";

    private readonly Mock<UserManager<User>> _userManagerMock = new(
        Mock.Of<IUserStore<User>>(), null!, null!, null!, null!, null!, null!, null!, null!);

    private readonly User _user = new() { Id = Guid.NewGuid().ToString(), UserName = "jkowalska" };
    private readonly AuthenticationService _sut;

    public AuthenticationServiceTests()
    {
        var jwtConfiguration = Options.Create(new JwtConfiguration
        {
            ValidIssuer = "ReceptionBookAPI",
            ValidAudience = "ReceptionBookClient",
            SecretKey = "TestSecretKeyForReceptionBookUnitTests_1234567890",
            ExpiresInMinutes = 15,
            RefreshTokenExpiresInDays = 7
        });

        _userManagerMock.Setup(m => m.FindByNameAsync(_user.UserName!)).ReturnsAsync(_user);
        _userManagerMock.Setup(m => m.GetRolesAsync(_user)).ReturnsAsync(new List<string> { UserRoles.Receptionist });
        _userManagerMock.Setup(m => m.UpdateAsync(_user)).ReturnsAsync(IdentityResult.Success);
        _userManagerMock.Setup(m => m.ResetAccessFailedCountAsync(_user)).ReturnsAsync(IdentityResult.Success);

        _sut = new AuthenticationService(Mock.Of<ILoggerManager>(), TestMapper.Create(), _userManagerMock.Object,
            jwtConfiguration);
    }

    [Fact]
    public async Task ValidateUser_WhenUserDoesNotExist_ReturnsFalse()
    {
        var result = await _sut.ValidateUser(Credentials("unknown"));

        result.Should().BeFalse();
    }

    [Fact]
    public async Task ValidateUser_WhenAccountIsLockedOut_ReturnsFalseWithoutCheckingPassword()
    {
        _userManagerMock.Setup(m => m.IsLockedOutAsync(_user)).ReturnsAsync(true);

        var result = await _sut.ValidateUser(Credentials());

        result.Should().BeFalse();
        _userManagerMock.Verify(m => m.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task ValidateUser_WhenPasswordIsWrong_RegistersFailedAttempt()
    {
        _userManagerMock.Setup(m => m.CheckPasswordAsync(_user, It.IsAny<string>())).ReturnsAsync(false);

        var result = await _sut.ValidateUser(Credentials());

        result.Should().BeFalse();
        _userManagerMock.Verify(m => m.AccessFailedAsync(_user), Times.Once);
    }

    [Fact]
    public async Task ValidateUser_WhenPasswordIsCorrect_ResetsFailedAttempts()
    {
        _userManagerMock.Setup(m => m.CheckPasswordAsync(_user, Password)).ReturnsAsync(true);

        var result = await _sut.ValidateUser(Credentials());

        result.Should().BeTrue();
        _userManagerMock.Verify(m => m.ResetAccessFailedCountAsync(_user), Times.Once);
    }

    [Fact]
    public async Task CreateToken_ReturnsAccessTokenWithRolesAndStoresHashedRefreshToken()
    {
        var tokenDto = await LogIn();

        var accessToken = new JwtSecurityTokenHandler().ReadJwtToken(tokenDto.AccessToken);
        accessToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == UserRoles.Receptionist);
        accessToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Name && c.Value == _user.UserName);

        _user.RefreshToken.Should().NotBe(tokenDto.RefreshToken);
        _user.RefreshToken.Should().Be(Hash(tokenDto.RefreshToken));
        _user.RefreshTokenExpiryTime.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public async Task RefreshToken_WithValidTokens_RotatesRefreshToken()
    {
        var tokenDto = await LogIn();

        var refreshed = await _sut.RefreshToken(tokenDto);

        refreshed.RefreshToken.Should().NotBe(tokenDto.RefreshToken);
        _user.RefreshToken.Should().Be(Hash(refreshed.RefreshToken));
    }

    [Fact]
    public async Task RefreshToken_WhenRefreshTokenDoesNotMatch_ThrowsBadRequestException()
    {
        var tokenDto = await LogIn();

        var act = () => _sut.RefreshToken(tokenDto with { RefreshToken = "forged-refresh-token" });

        await act.Should().ThrowAsync<RefreshTokenBadRequestException>();
    }

    [Fact]
    public async Task RefreshToken_WhenAccessTokenIsTampered_ThrowsBadRequestException()
    {
        var tokenDto = await LogIn();

        var act = () => _sut.RefreshToken(tokenDto with { AccessToken = tokenDto.AccessToken + "x" });

        await act.Should().ThrowAsync<RefreshTokenBadRequestException>();
    }

    [Fact]
    public async Task RegisterUser_WithUnknownRole_ReturnsFailedResult()
    {
        var registration = new UserForRegistrationDto
        {
            UserName = "intruder",
            Password = Password,
            Roles = new List<string> { "SuperAdmin" }
        };

        var result = await _sut.RegisterUser(registration);

        result.Succeeded.Should().BeFalse();
        _userManagerMock.Verify(m => m.CreateAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
    }

    private async Task<TokenDto> LogIn()
    {
        _userManagerMock.Setup(m => m.CheckPasswordAsync(_user, Password)).ReturnsAsync(true);
        (await _sut.ValidateUser(Credentials())).Should().BeTrue();

        return await _sut.CreateToken(populateExp: true);
    }

    private UserForAuthenticationDto Credentials(string? userName = null) =>
        new() { UserName = userName ?? _user.UserName, Password = Password };

    private static string Hash(string token) =>
        Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
}
