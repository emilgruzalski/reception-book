using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects;

public record PasswordChangeDto
{
    [Required(ErrorMessage = "Current password is required.")]
    public string? CurrentPassword { get; init; }

    [Required(ErrorMessage = "New password is required.")]
    public string? NewPassword { get; init; }

    [Required(ErrorMessage = "Password confirmation is required.")]
    [Compare(nameof(NewPassword), ErrorMessage = "The new password and confirmation do not match.")]
    public string? ConfirmNewPassword { get; init; }
}
