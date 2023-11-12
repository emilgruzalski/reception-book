using System.ComponentModel.DataAnnotations;

namespace Shared.RequestFeatures;

public class RoomAvailabilityParameters
{
    [Required]
    public DateTime? CheckInDate { get; set; }

    [Required]
    public DateTime? CheckOutDate { get; set; }

    [Range(1, 10)]
    public int NumberOfGuests { get; set; } = 1;
}
