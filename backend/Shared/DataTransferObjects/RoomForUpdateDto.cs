using Entities.Models;

namespace Shared.DataTransferObjects;

public record RoomForUpdateDto : RoomForManipulationDto
{
    public RoomStatus Status { get; init; }
}
