using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace ReceptionBook.Presentation.Controllers;

[Route("api/guests")]
[ApiController]
public class GuestsController : ControllerBase
{
    private readonly IServiceManager _service;

    public GuestsController(IServiceManager service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetGuests()
    {
        var guests = await _service.GuestService.GetAllGuestsAsync(trackChanges: false);

        return Ok(guests);
    }

    [HttpGet("{id:guid}", Name = "GuestById")]
    public async Task<IActionResult> GetGuest(Guid id)
    {
        var guest = await _service.GuestService.GetGuestAsync(id, trackChanges: false);

        return Ok(guest);
    }

    [HttpPost]
    public async Task<IActionResult> CreateGuest([FromBody] GuestForCreationDto guest)
    {
        if (guest is null)
            return BadRequest("GuestForCreationDto object is null");

        var createdGuest = await _service.GuestService.CreateGuestAsync(guest);

        return CreatedAtRoute("GuestById", new { id = createdGuest.Id }, createdGuest);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateGuest(Guid id, [FromBody] GuestForUpdateDto guest)
    {
        if (guest is null)
            return BadRequest("GuestForUpdateDto object is null");

        await _service.GuestService.UpdateGuestAsync(id, guest, trackChanges: true);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteGuest(Guid id)
    {
        await _service.GuestService.DeleteGuestAsync(id, trackChanges: false);

        return NoContent();
    }
}
