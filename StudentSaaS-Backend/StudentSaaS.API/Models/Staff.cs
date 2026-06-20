namespace StudentSaaS.API.Models;

using System;

public class Staff
{
    public int Id { get; set; }

    public int InstituteId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty; // e.g. "Teacher", "Admin"

    public string Phone { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Subject { get; set; } = string.Empty;

    public DateTime JoiningDate { get; set; } = DateTime.UtcNow;

    public string Designation { get; set; } = string.Empty;

    public decimal Salary { get; set; }

    public bool ActiveStatus { get; set; } = true;
    public bool IsDeleted { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
}