namespace StudentSaaS.API.Models;

using System;
using System.ComponentModel.DataAnnotations;

public class Course
{
    public int Id { get; set; }

    [Required]
    public string CourseName { get; set; } = string.Empty;

    [Required]
    public string CourseCode { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    public decimal Fee { get; set; }

    public int DurationMonths { get; set; }

    public string Category { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public int InstituteId { get; set; }

    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
}