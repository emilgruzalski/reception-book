using Microsoft.AspNetCore.Mvc;
using Service.Contracts;

namespace ReceptionBook.Presentation.Controllers;

[Route("api/rooms")]
[ApiController]
public class RoomsController : ControllerBase
{
    private readonly IServiceManager _service;

    public RoomsController(IServiceManager service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetRooms()
    {
        var rooms = await _service.RoomService.GetAllRoomsAsync(trackChanges: false);

        return Ok(rooms);
    }

    [HttpGet("{id:guid}", Name = "RoomById")]
    public async Task<IActionResult> GetRoom(Guid id)
    {
        var room = await _service.RoomService.GetRoomAsync(id, trackChanges: false);
        if (room is null)
            return NotFound();

        return Ok(room);
    }
}
