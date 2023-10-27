namespace Shared.DataTransferObjects;

public abstract record GuestForManipulationDto
{
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? Email { get; init; }
    public string? PhoneNumber { get; init; }
    public string? DocumentNumber { get; init; }
    public string? Address { get; init; }
}
