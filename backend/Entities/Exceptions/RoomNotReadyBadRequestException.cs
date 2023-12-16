using Entities.Models;

namespace Entities.Exceptions;

public sealed class RoomNotReadyBadRequestException : BadRequestException
{
    public RoomNotReadyBadRequestException(string roomNumber, RoomStatus status)
        : base($"Room {roomNumber} is not ready for check-in. Current status: {status}.")
    {
    }
}
