using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Contracts;
using Entities.ConfigurationModels;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Service;

internal sealed class AuthenticationService : IAuthenticationService
{
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

        var result = _user != null && await _userManager.CheckPasswordAsync(_user, userForAuth.Password!);
        if (!result)
            _logger.LogWarn($"{nameof(ValidateUser)}: Authentication failed. Wrong user name or password.");

        return result;
    }

    public async Task<string> CreateToken()
    {
        var signingCredentials = GetSigningCredentials();
        var claims = await GetClaims();
        var tokenOptions = GenerateTokenOptions(signingCredentials, claims);

        return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
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
}
