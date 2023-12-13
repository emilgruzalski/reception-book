using Microsoft.AspNetCore.Identity;
using Shared.DataTransferObjects;

namespace Service.Contracts;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetUsersAsync();
    Task<UserDto> GetUserAsync(string userId);
    Task<IdentityResult> UpdateUserAsync(string userId, UserForUpdateDto userForUpdate, string currentUserName);
    Task UnlockUserAsync(string userId);
    Task DeleteUserAsync(string userId, string currentUserName);
}
