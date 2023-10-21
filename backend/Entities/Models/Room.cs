using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models;

public class Room
{
    [Column("RoomId")]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Room number is a required field.")]
    [MaxLength(10, ErrorMessage = "Maximum length for the room number is 10 characters.")]
    public string? Number { get; set; }

    public int Floor { get; set; }

    public RoomType Type { get; set; }

    public int Capacity { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal PricePerNight { get; set; }

    public RoomStatus Status { get; set; }

    [MaxLength(500, ErrorMessage = "Maximum length for the description is 500 characters.")]
    public string? Description { get; set; }

    public ICollection<Reservation>? Reservations { get; set; }
}
