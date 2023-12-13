namespace Entities.Exceptions;

public sealed class OwnAccountModificationBadRequestException : BadRequestException
{
    public OwnAccountModificationBadRequestException(string action)
        : base($"You cannot {action} your own account.")
    {
    }
}
