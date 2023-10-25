using Entities.Models;

namespace Shared.DataTransferObjects;

public abstract record RoomForManipulationDto
{
    public string? Number { get; init; }
    public int Floor { get; init; }
    public RoomType Type { get; init; }
    public int Capacity { get; init; }
    public decimal PricePerNight { get; init; }
    public string? Description { get; init; }
}
