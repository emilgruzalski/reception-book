namespace Entities.Exceptions;

public sealed class DateRangeBadRequestException : BadRequestException
{
    public DateRangeBadRequestException()
        : base("End date can't be earlier than start date.")
    {
    }
}
