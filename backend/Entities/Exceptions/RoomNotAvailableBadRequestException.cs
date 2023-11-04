namespace Entities.Exceptions;

public sealed class RoomNotAvailableBadRequestException : BadRequestException
{
    public RoomNotAvailableBadRequestException(string roomNumber)
        : base($"Room {roomNumber} is already reserved in the selected period.")
    {
    }
}
