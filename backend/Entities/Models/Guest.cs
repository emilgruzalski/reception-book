using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models;

public class Guest
{
    [Column("GuestId")]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "First name is a required field.")]
    [MaxLength(50, ErrorMessage = "Maximum length for the first name is 50 characters.")]
    public string? FirstName { get; set; }

    [Required(ErrorMessage = "Last name is a required field.")]
    [MaxLength(50, ErrorMessage = "Maximum length for the last name is 50 characters.")]
    public string? LastName { get; set; }

    [MaxLength(100, ErrorMessage = "Maximum length for the email is 100 characters.")]
    public string? Email { get; set; }

    [MaxLength(20, ErrorMessage = "Maximum length for the phone number is 20 characters.")]
    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "Document number is a required field.")]
    [MaxLength(30, ErrorMessage = "Maximum length for the document number is 30 characters.")]
    public string? DocumentNumber { get; set; }

    [MaxLength(200, ErrorMessage = "Maximum length for the address is 200 characters.")]
    public string? Address { get; set; }

    public DateTime CreatedAt { get; set; }

    public ICollection<Reservation>? Reservations { get; set; }
}
