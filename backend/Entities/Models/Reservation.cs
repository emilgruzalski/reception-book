using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models;

public class Reservation
{
    [Column("ReservationId")]
    public Guid Id { get; set; }

    [Column(TypeName = "date")]
    public DateTime CheckInDate { get; set; }

    [Column(TypeName = "date")]
    public DateTime CheckOutDate { get; set; }

    public int NumberOfGuests { get; set; }

    public ReservationStatus Status { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal TotalPrice { get; set; }

    [MaxLength(500, ErrorMessage = "Maximum length for the notes is 500 characters.")]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    [ForeignKey(nameof(Room))]
    public Guid RoomId { get; set; }
    public Room? Room { get; set; }

    [ForeignKey(nameof(Guest))]
    public Guid GuestId { get; set; }
    public Guest? Guest { get; set; }
}
