using System.ComponentModel.DataAnnotations;

namespace StudentSaaS.API.DTOs;

public class ResetPasswordDto
{
    [Required]
    public string Token { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string NewPassword { get; set; } = string.Empty;

    [Required]
    [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
    public string ConfirmNewPassword { get; set; } = string.Empty;
}
