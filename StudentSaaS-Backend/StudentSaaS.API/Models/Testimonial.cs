using System;

namespace StudentSaaS.API.Models;

public class Testimonial
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public int Rating { get; set; } = 5;

    public string ImageUrl { get; set; } = string.Empty;

    public int InstituteId { get; set; }

    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
}
