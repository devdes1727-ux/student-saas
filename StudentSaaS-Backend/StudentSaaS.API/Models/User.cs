using System;
using System.Collections.Generic;
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

    public string Phone { get; set; } = string.Empty;

    public string? ResetToken { get; set; }

    public DateTime? ResetTokenExpiry { get; set; }

    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }

    public List<RefreshToken> RefreshTokens { get; set; } = new();
}