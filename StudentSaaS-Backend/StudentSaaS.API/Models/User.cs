using System.ComponentModel.DataAnnotations;

namespace StudentSaaS.API.Models;

public class User
{
    public int Id { get; set; }

    public int? InstituteId { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    public string Role { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}