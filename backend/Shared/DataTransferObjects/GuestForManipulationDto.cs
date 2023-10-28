using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects;

public abstract record GuestForManipulationDto
{
    [Required(ErrorMessage = "First name is a required field.")]
    [MaxLength(50, ErrorMessage = "Maximum length for the first name is 50 characters.")]
    public string? FirstName { get; init; }

    [Required(ErrorMessage = "Last name is a required field.")]
    [MaxLength(50, ErrorMessage = "Maximum length for the last name is 50 characters.")]
    public string? LastName { get; init; }

    [EmailAddress(ErrorMessage = "Email address is not valid.")]
    [MaxLength(100, ErrorMessage = "Maximum length for the email is 100 characters.")]
    public string? Email { get; init; }

    [Phone(ErrorMessage = "Phone number is not valid.")]
    [MaxLength(20, ErrorMessage = "Maximum length for the phone number is 20 characters.")]
    public string? PhoneNumber { get; init; }

    [Required(ErrorMessage = "Document number is a required field.")]
    [MaxLength(30, ErrorMessage = "Maximum length for the document number is 30 characters.")]
    public string? DocumentNumber { get; init; }

    [MaxLength(200, ErrorMessage = "Maximum length for the address is 200 characters.")]
    public string? Address { get; init; }
}
