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

[Route("api/rooms")]
[ApiController]
[Authorize]
public class RoomsController : ControllerBase
{
    private readonly IServiceManager _service;

    public RoomsController(IServiceManager service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetRooms([FromQuery] RoomParameters roomParameters)
    {
        var pagedResult = await _service.RoomService.GetAllRoomsAsync(roomParameters, trackChanges: false);

        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(pagedResult.metaData));

        return Ok(pagedResult.rooms);
    }

    [HttpGet("available")]
    public async Task<IActionResult> GetAvailableRooms([FromQuery] RoomAvailabilityParameters parameters)
    {
        if (!ModelState.IsValid)
            return UnprocessableEntity(ModelState);

        var rooms = await _service.RoomService.GetAvailableRoomsAsync(parameters, trackChanges: false);

        return Ok(rooms);
    }

    [HttpGet("{id:guid}", Name = "RoomById")]
    public async Task<IActionResult> GetRoom(Guid id)
    {
        var room = await _service.RoomService.GetRoomAsync(id, trackChanges: false);

        return Ok(room);
    }

    [HttpPost]
    [Authorize(Roles = UserRoles.Administrator)]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> CreateRoom([FromBody] RoomForCreationDto room)
    {
        var createdRoom = await _service.RoomService.CreateRoomAsync(room);

        return CreatedAtRoute("RoomById", new { id = createdRoom.Id }, createdRoom);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = UserRoles.Administrator)]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> UpdateRoom(Guid id, [FromBody] RoomForUpdateDto room)
    {
        await _service.RoomService.UpdateRoomAsync(id, room, trackChanges: true);

        return NoContent();
    }

    [HttpPut("{id:guid}/status")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> UpdateRoomStatus(Guid id, [FromBody] RoomStatusForUpdateDto roomStatus)
    {
        await _service.RoomService.UpdateRoomStatusAsync(id, roomStatus, trackChanges: true);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = UserRoles.Administrator)]
    public async Task<IActionResult> DeleteRoom(Guid id)
    {
        await _service.RoomService.DeleteRoomAsync(id, trackChanges: false);

        return NoContent();
    }
}
