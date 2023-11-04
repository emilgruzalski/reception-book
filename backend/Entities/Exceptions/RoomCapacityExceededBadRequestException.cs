namespace Entities.Exceptions;

public sealed class RoomCapacityExceededBadRequestException : BadRequestException
{
    public RoomCapacityExceededBadRequestException(string roomNumber, int capacity)
        : base($"Room {roomNumber} can accommodate at most {capacity} guest(s).")
    {
    }
}
