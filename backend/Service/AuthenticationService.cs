using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Contracts;
using Entities.ConfigurationModels;
using Entities.Exceptions;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Service;

internal sealed class AuthenticationService : IAuthenticationService
{
    private const int RefreshTokenSizeInBytes = 64;

    private readonly ILoggerManager _logger;
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;
    private readonly JwtConfiguration _jwtConfiguration;

    private User? _user;

    public AuthenticationService(ILoggerManager logger, IMapper mapper, UserManager<User> userManager,
        IOptions<JwtConfiguration> configuration)
    {
        _logger = logger;
        _mapper = mapper;
        _userManager = userManager;
        _jwtConfiguration = configuration.Value;
    }

    public async Task<IdentityResult> RegisterUser(UserForRegistrationDto userForRegistration)
    {
        var invalidRoles = userForRegistration.Roles!.Except(UserRoles.All).ToList();
        if (invalidRoles.Any())
        {
            return IdentityResult.Failed(new IdentityError
            {
                Code = "InvalidRole",
                Description = $"Invalid role(s): {string.Join(", ", invalidRoles)}."
            });
        }

        var user = _mapper.Map<User>(userForRegistration);

        var result = await _userManager.CreateAsync(user, userForRegistration.Password!);
        if (result.Succeeded)
            await _userManager.AddToRolesAsync(user, userForRegistration.Roles!);

        return result;
    }

    public async Task<bool> ValidateUser(UserForAuthenticationDto userForAuth)
    {
        _user = await _userManager.FindByNameAsync(userForAuth.UserName!);
        if (_user == null)
        {
            _logger.LogWarn($"{nameof(ValidateUser)}: Authentication failed. Wrong user name or password.");
            return false;
        }

        if (await _userManager.IsLockedOutAsync(_user))
        {
            _logger.LogWarn($"{nameof(ValidateUser)}: Authentication failed. Account '{_user.UserName}' is locked out.");
            return false;
        }

        if (!await _userManager.CheckPasswordAsync(_user, userForAuth.Password!))
        {
            // Count failed attempts so that the account gets locked out after too many tries.
            await _userManager.AccessFailedAsync(_user);
            _logger.LogWarn($"{nameof(ValidateUser)}: Authentication failed. Wrong user name or password.");
            return false;
        }

        await _userManager.ResetAccessFailedCountAsync(_user);

        return true;
    }

    public async Task<TokenDto> CreateToken(bool populateExp)
    {
        var signingCredentials = GetSigningCredentials();
        var claims = await GetClaims();
        var tokenOptions = GenerateTokenOptions(signingCredentials, claims);

        var refreshToken = GenerateRefreshToken();

        // Only a hash of the refresh token is stored, so a database leak does not expose valid tokens.
        _user!.RefreshToken = HashToken(refreshToken);
        if (populateExp)
            _user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtConfiguration.RefreshTokenExpiresInDays);

        await _userManager.UpdateAsync(_user);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

        return new TokenDto(accessToken, refreshToken);
    }

    public async Task<TokenDto> RefreshToken(TokenDto tokenDto)
    {
        var principal = GetPrincipalFromExpiredToken(tokenDto.AccessToken);

        var user = await _userManager.FindByNameAsync(principal.Identity!.Name!);
        if (user == null || !IsRefreshTokenValid(user, tokenDto.RefreshToken))
            throw new RefreshTokenBadRequestException();

        _user = user;

        return await CreateToken(populateExp: false);
    }

    public async Task RevokeRefreshToken(string userName)
    {
        var user = await _userManager.FindByNameAsync(userName);
        if (user is null)
            return;

        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = DateTime.MinValue;

        await _userManager.UpdateAsync(user);
    }

    public async Task<IdentityResult> ChangePassword(string userName, PasswordChangeDto passwordChange)
    {
        var user = await _userManager.FindByNameAsync(userName);
        if (user is null)
            throw new UserNotFoundException(userName);

        var result = await _userManager.ChangePasswordAsync(user, passwordChange.CurrentPassword!,
            passwordChange.NewPassword!);
        if (!result.Succeeded)
            return result;

        // Sign out other sessions by invalidating the stored refresh token.
        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = DateTime.MinValue;
        await _userManager.UpdateAsync(user);

        _logger.LogInfo($"User '{userName}' has changed the password.");

        return result;
    }

    private SigningCredentials GetSigningCredentials()
    {
        var key = Encoding.UTF8.GetBytes(_jwtConfiguration.SecretKey!);
        var secret = new SymmetricSecurityKey(key);

        return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
    }

    private async Task<List<Claim>> GetClaims()
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, _user!.Id),
            new(ClaimTypes.Name, _user.UserName!),
            new(ClaimTypes.GivenName, _user.FirstName ?? string.Empty),
            new(ClaimTypes.Surname, _user.LastName ?? string.Empty)
        };

        var roles = await _userManager.GetRolesAsync(_user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        return claims;
    }

    private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims) =>
        new(
            issuer: _jwtConfiguration.ValidIssuer,
            audience: _jwtConfiguration.ValidAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtConfiguration.ExpiresInMinutes),
            signingCredentials: signingCredentials);

    private static string HashToken(string token) =>
        Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(token)));

    private static bool IsRefreshTokenValid(User user, string refreshToken)
    {
        if (string.IsNullOrEmpty(user.RefreshToken) || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            return false;

        var storedHash = Encoding.UTF8.GetBytes(user.RefreshToken);
        var providedHash = Encoding.UTF8.GetBytes(HashToken(refreshToken));

        // Constant-time comparison prevents timing attacks on the token value.
        return CryptographicOperations.FixedTimeEquals(storedHash, providedHash);
    }

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[RefreshTokenSizeInBytes];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);

        return Convert.ToBase64String(randomNumber);
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfiguration.SecretKey!)),
            ValidateLifetime = false,
            ValidIssuer = _jwtConfiguration.ValidIssuer,
            ValidAudience = _jwtConfiguration.ValidAudience
        };

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase))
                throw new RefreshTokenBadRequestException();

            return principal;
        }
        catch (Exception ex) when (ex is SecurityTokenException or ArgumentException)
        {
            throw new RefreshTokenBadRequestException();
        }
    }
}
