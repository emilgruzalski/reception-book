namespace Entities.Exceptions;

public sealed class RoomHasReservationsBadRequestException : BadRequestException
{
    public RoomHasReservationsBadRequestException(Guid roomId)
        : base($"The room with id: {roomId} has reservations and cannot be deleted.")
    {
    }
}
