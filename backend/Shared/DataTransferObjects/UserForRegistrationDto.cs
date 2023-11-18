using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects;

public record UserForRegistrationDto
{
    [Required(ErrorMessage = "First name is a required field.")]
    [MaxLength(50, ErrorMessage = "Maximum length for the first name is 50 characters.")]
    public string? FirstName { get; init; }

    [Required(ErrorMessage = "Last name is a required field.")]
    [MaxLength(50, ErrorMessage = "Maximum length for the last name is 50 characters.")]
    public string? LastName { get; init; }

    [Required(ErrorMessage = "Username is required.")]
    public string? UserName { get; init; }

    [Required(ErrorMessage = "Password is required.")]
    public string? Password { get; init; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Email address is not valid.")]
    public string? Email { get; init; }

    [Phone(ErrorMessage = "Phone number is not valid.")]
    public string? PhoneNumber { get; init; }

    [Required(ErrorMessage = "At least one role is required.")]
    public ICollection<string>? Roles { get; init; }
}
