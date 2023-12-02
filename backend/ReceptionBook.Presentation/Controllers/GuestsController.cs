using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReceptionBook.Presentation.ActionFilters;
using Service.Contracts;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;
using System.Text.Json;

namespace ReceptionBook.Presentation.Controllers;

[Route("api/guests")]
[ApiController]
[Authorize]
public class GuestsController : ControllerBase
{
    private readonly IServiceManager _service;

    public GuestsController(IServiceManager service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetGuests([FromQuery] GuestParameters guestParameters)
    {
        var pagedResult = await _service.GuestService.GetAllGuestsAsync(guestParameters, trackChanges: false);

        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(pagedResult.metaData));

        return Ok(pagedResult.guests);
    }

    [HttpGet("{id:guid}", Name = "GuestById")]
    public async Task<IActionResult> GetGuest(Guid id)
    {
        var guest = await _service.GuestService.GetGuestAsync(id, trackChanges: false);

        return Ok(guest);
    }

    [HttpPost]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> CreateGuest([FromBody] GuestForCreationDto guest)
    {
        var createdGuest = await _service.GuestService.CreateGuestAsync(guest);

        return CreatedAtRoute("GuestById", new { id = createdGuest.Id }, createdGuest);
    }

    [HttpPut("{id:guid}")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> UpdateGuest(Guid id, [FromBody] GuestForUpdateDto guest)
    {
        await _service.GuestService.UpdateGuestAsync(id, guest, trackChanges: true);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = UserRoles.Administrator)]
    public async Task<IActionResult> DeleteGuest(Guid id)
    {
        await _service.GuestService.DeleteGuestAsync(id, trackChanges: false);

        return NoContent();
    }
}
