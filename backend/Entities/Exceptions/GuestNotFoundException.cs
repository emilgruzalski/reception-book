namespace Entities.Exceptions;

public sealed class GuestNotFoundException : NotFoundException
{
    public GuestNotFoundException(Guid guestId)
        : base($"The guest with id: {guestId} doesn't exist in the database.")
    {
    }
}
