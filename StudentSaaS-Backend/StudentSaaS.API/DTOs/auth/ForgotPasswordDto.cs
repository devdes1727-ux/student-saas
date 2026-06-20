using System.ComponentModel.DataAnnotations;

namespace StudentSaaS.API.DTOs;

public class ForgotPasswordDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}
