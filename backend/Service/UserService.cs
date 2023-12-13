using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Service;

internal sealed class UserService : IUserService
{
    private readonly ILoggerManager _logger;
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;

    public UserService(ILoggerManager logger, IMapper mapper, UserManager<User> userManager)
    {
        _logger = logger;
        _mapper = mapper;
        _userManager = userManager;
    }

    public async Task<IEnumerable<UserDto>> GetUsersAsync()
    {
        var users = await _userManager.Users
            .AsNoTracking()
            .OrderBy(u => u.LastName)
            .ThenBy(u => u.FirstName)
            .ToListAsync();

        var usersDto = new List<UserDto>();
        foreach (var user in users)
            usersDto.Add(await MapToDto(user));

        return usersDto;
    }

    public async Task<UserDto> GetUserAsync(string userId)
    {
        var user = await GetUserAndCheckIfItExists(userId);

        return await MapToDto(user);
    }

    public async Task<IdentityResult> UpdateUserAsync(string userId, UserForUpdateDto userForUpdate,
        string currentUserName)
    {
        var invalidRoles = userForUpdate.Roles!.Except(UserRoles.All).ToList();
        if (invalidRoles.Any())
        {
            return IdentityResult.Failed(new IdentityError
            {
                Code = "InvalidRole",
                Description = $"Invalid role(s): {string.Join(", ", invalidRoles)}."
            });
        }

        var user = await GetUserAndCheckIfItExists(userId);

        if (user.UserName == currentUserName && !userForUpdate.Roles!.Contains(UserRoles.Administrator))
            throw new OwnAccountModificationBadRequestException("remove the administrator role from");

        _mapper.Map(userForUpdate, user);

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return result;

        var currentRoles = await _userManager.GetRolesAsync(user);

        result = await _userManager.RemoveFromRolesAsync(user, currentRoles.Except(userForUpdate.Roles!));
        if (!result.Succeeded)
            return result;

        result = await _userManager.AddToRolesAsync(user, userForUpdate.Roles!.Except(currentRoles));
        if (result.Succeeded)
            _logger.LogInfo($"User '{user.UserName}' has been updated by '{currentUserName}'.");

        return result;
    }

    public async Task UnlockUserAsync(string userId)
    {
        var user = await GetUserAndCheckIfItExists(userId);

        await _userManager.SetLockoutEndDateAsync(user, null);
        await _userManager.ResetAccessFailedCountAsync(user);

        _logger.LogInfo($"User '{user.UserName}' has been unlocked.");
    }

    public async Task DeleteUserAsync(string userId, string currentUserName)
    {
        var user = await GetUserAndCheckIfItExists(userId);

        if (user.UserName == currentUserName)
            throw new OwnAccountModificationBadRequestException("delete");

        await _userManager.DeleteAsync(user);

        _logger.LogInfo($"User '{user.UserName}' has been deleted by '{currentUserName}'.");
    }

    private async Task<UserDto> MapToDto(User user)
    {
        var userDto = _mapper.Map<UserDto>(user);
        userDto.Roles = await _userManager.GetRolesAsync(user);
        userDto.IsLockedOut = await _userManager.IsLockedOutAsync(user);

        return userDto;
    }

    private async Task<User> GetUserAndCheckIfItExists(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            throw new UserNotFoundException(userId);

        return user;
    }
}
