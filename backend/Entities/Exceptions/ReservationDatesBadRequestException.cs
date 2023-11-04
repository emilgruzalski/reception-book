namespace Entities.Exceptions;

public sealed class ReservationDatesBadRequestException : BadRequestException
{
    public ReservationDatesBadRequestException()
        : base("Check-out date must be later than check-in date.")
    {
    }
}
