using Entities.Models;

namespace Shared.DataTransferObjects;

public record RoomDto
{
    public Guid Id { get; init; }
    public string? Number { get; init; }
    public int Floor { get; init; }
    public RoomType Type { get; init; }
    public int Capacity { get; init; }
    public decimal PricePerNight { get; init; }
    public RoomStatus Status { get; init; }
    public string? Description { get; init; }
}
