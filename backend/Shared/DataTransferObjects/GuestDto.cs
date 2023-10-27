namespace Shared.DataTransferObjects;

public record GuestDto
{
    public Guid Id { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? Email { get; init; }
    public string? PhoneNumber { get; init; }
    public string? DocumentNumber { get; init; }
    public string? Address { get; init; }
    public DateTime CreatedAt { get; init; }
}
