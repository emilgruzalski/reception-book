namespace Entities.Exceptions;

public sealed class CheckInDateBadRequestException : BadRequestException
{
    public CheckInDateBadRequestException(DateTime checkInDate)
        : base($"Check-in is possible only from {checkInDate:yyyy-MM-dd} until the check-out date.")
    {
    }
}
