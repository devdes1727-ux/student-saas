namespace StudentSaaS.API.Models;

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

    public bool IsActive { get; set; } = true;

    public int InstituteId { get; set; }
}