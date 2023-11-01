namespace Entities.Exceptions;

public sealed class RoomNumberAlreadyExistsBadRequestException : BadRequestException
{
    public RoomNumberAlreadyExistsBadRequestException(string roomNumber)
        : base($"Room with number {roomNumber} already exists.")
    {
    }
}
