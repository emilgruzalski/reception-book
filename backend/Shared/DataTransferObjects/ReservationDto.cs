using Entities.Models;

namespace Shared.DataTransferObjects;

public record ReservationDto
{
    public Guid Id { get; init; }
    public Guid RoomId { get; init; }
    public string? RoomNumber { get; init; }
    public Guid GuestId { get; init; }
    public string? GuestFullName { get; init; }
    public DateTime CheckInDate { get; init; }
    public DateTime CheckOutDate { get; init; }
    public int NumberOfGuests { get; init; }
    public ReservationStatus Status { get; init; }
    public decimal TotalPrice { get; init; }
    public string? Notes { get; init; }
    public DateTime CreatedAt { get; init; }
}
