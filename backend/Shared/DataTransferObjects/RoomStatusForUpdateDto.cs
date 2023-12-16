using System.ComponentModel.DataAnnotations;
using Entities.Models;

namespace Shared.DataTransferObjects;

public record RoomStatusForUpdateDto
{
    [Required(ErrorMessage = "Status is a required field.")]
    [EnumDataType(typeof(RoomStatus), ErrorMessage = "Room status is not valid.")]
    public RoomStatus? Status { get; init; }
}
