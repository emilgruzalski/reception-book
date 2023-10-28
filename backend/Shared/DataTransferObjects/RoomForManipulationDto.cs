using System.ComponentModel.DataAnnotations;
using Entities.Models;

namespace Shared.DataTransferObjects;

public abstract record RoomForManipulationDto
{
    [Required(ErrorMessage = "Room number is a required field.")]
    [MaxLength(10, ErrorMessage = "Maximum length for the room number is 10 characters.")]
    public string? Number { get; init; }

    [Range(0, 50, ErrorMessage = "Floor must be between 0 and 50.")]
    public int Floor { get; init; }

    [EnumDataType(typeof(RoomType), ErrorMessage = "Room type is not valid.")]
    public RoomType Type { get; init; }

    [Range(1, 10, ErrorMessage = "Capacity must be between 1 and 10.")]
    public int Capacity { get; init; }

    [Range(typeof(decimal), "1", "100000", ErrorMessage = "Price per night must be between 1 and 100000.")]
    public decimal PricePerNight { get; init; }

    [MaxLength(500, ErrorMessage = "Maximum length for the description is 500 characters.")]
    public string? Description { get; init; }
}
