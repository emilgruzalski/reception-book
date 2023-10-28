using System.ComponentModel.DataAnnotations;
using Entities.Models;

namespace Shared.DataTransferObjects;

public record RoomForUpdateDto : RoomForManipulationDto
{
    [EnumDataType(typeof(RoomStatus), ErrorMessage = "Room status is not valid.")]
    public RoomStatus Status { get; init; }
}
