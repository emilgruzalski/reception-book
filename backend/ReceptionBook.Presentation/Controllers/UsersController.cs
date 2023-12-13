using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReceptionBook.Presentation.ActionFilters;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace ReceptionBook.Presentation.Controllers;

[Route("api/users")]
[ApiController]
[Authorize(Roles = UserRoles.Administrator)]
public class UsersController : ControllerBase
{
    private readonly IServiceManager _service;

    public UsersController(IServiceManager service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _service.UserService.GetUsersAsync();

        return Ok(users);
    }

    [HttpGet("{id}", Name = "UserById")]
    public async Task<IActionResult> GetUser(string id)
    {
        var user = await _service.UserService.GetUserAsync(id);

        return Ok(user);
    }

    [HttpPut("{id}")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] UserForUpdateDto user)
    {
        var result = await _service.UserService.UpdateUserAsync(id, user, User.Identity!.Name!);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.TryAddModelError(error.Code, error.Description);

            return BadRequest(ModelState);
        }

        return NoContent();
    }

    [HttpPost("{id}/unlock")]
    public async Task<IActionResult> UnlockUser(string id)
    {
        await _service.UserService.UnlockUserAsync(id);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        await _service.UserService.DeleteUserAsync(id, User.Identity!.Name!);

        return NoContent();
    }
}
