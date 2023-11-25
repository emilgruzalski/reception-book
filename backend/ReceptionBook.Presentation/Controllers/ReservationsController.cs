using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReceptionBook.Presentation.ActionFilters;
using Service.Contracts;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;
using System.Text.Json;

namespace ReceptionBook.Presentation.Controllers;

[Route("api/reservations")]
[ApiController]
[Authorize]
public class ReservationsController : ControllerBase
{
    private readonly IServiceManager _service;

    public ReservationsController(IServiceManager service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetReservations([FromQuery] ReservationParameters reservationParameters)
    {
        var pagedResult = await _service.ReservationService.GetAllReservationsAsync(reservationParameters, trackChanges: false);

        Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagedResult.metaData));

        return Ok(pagedResult.reservations);
    }

    [HttpGet("{id:guid}", Name = "ReservationById")]
    public async Task<IActionResult> GetReservation(Guid id)
    {
        var reservation = await _service.ReservationService.GetReservationAsync(id, trackChanges: false);

        return Ok(reservation);
    }

    [HttpPost]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> CreateReservation([FromBody] ReservationForCreationDto reservation)
    {
        var createdReservation = await _service.ReservationService.CreateReservationAsync(reservation);

        return CreatedAtRoute("ReservationById", new { id = createdReservation.Id }, createdReservation);
    }

    [HttpPut("{id:guid}")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> UpdateReservation(Guid id, [FromBody] ReservationForUpdateDto reservation)
    {
        await _service.ReservationService.UpdateReservationAsync(id, reservation, trackChanges: true);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = UserRoles.Administrator)]
    public async Task<IActionResult> DeleteReservation(Guid id)
    {
        await _service.ReservationService.DeleteReservationAsync(id, trackChanges: false);

        return NoContent();
    }
}
