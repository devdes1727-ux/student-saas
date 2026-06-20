using System.ComponentModel.DataAnnotations;

namespace StudentSaaS.API.DTOs;

public class RegisterDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Phone { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required]
    public string Role { get; set; } = "Student"; // Default to Student, can be Admin/Teacher/Student

    public int? InstituteId { get; set; }
}