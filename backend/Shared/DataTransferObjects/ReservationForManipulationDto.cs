using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects;

public abstract record ReservationForManipulationDto
{
    [Required(ErrorMessage = "Room is a required field.")]
    public Guid? RoomId { get; init; }

    [Required(ErrorMessage = "Guest is a required field.")]
    public Guid? GuestId { get; init; }

    [Required(ErrorMessage = "Check-in date is a required field.")]
    public DateTime? CheckInDate { get; init; }

    [Required(ErrorMessage = "Check-out date is a required field.")]
    public DateTime? CheckOutDate { get; init; }

    [Range(1, 10, ErrorMessage = "Number of guests must be between 1 and 10.")]
    public int NumberOfGuests { get; init; }

    [MaxLength(500, ErrorMessage = "Maximum length for the notes is 500 characters.")]
    public string? Notes { get; init; }
}
