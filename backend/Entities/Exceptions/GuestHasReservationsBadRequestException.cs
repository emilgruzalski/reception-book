namespace Entities.Exceptions;

public sealed class GuestHasReservationsBadRequestException : BadRequestException
{
    public GuestHasReservationsBadRequestException(Guid guestId)
        : base($"The guest with id: {guestId} has reservations and cannot be deleted.")
    {
    }
}
