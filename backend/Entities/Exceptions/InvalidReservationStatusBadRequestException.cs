using Entities.Models;

namespace Entities.Exceptions;

public sealed class InvalidReservationStatusBadRequestException : BadRequestException
{
    public InvalidReservationStatusBadRequestException(ReservationStatus status, string action)
        : base($"Cannot {action} a reservation with status {status}.")
    {
    }
}
