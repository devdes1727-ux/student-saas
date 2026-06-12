using System.ComponentModel.DataAnnotations;

namespace StudentSaaS.API.DTOs;

public class CreateCourseDto
{
    [Required]
    public string CourseName { get; set; } = string.Empty;

    [Required]
    public string CourseCode { get; set; } = string.Empty;

    [Range(1, 1000000)]
    public decimal Fee { get; set; }

    [Range(1, 120)]
    public int DurationMonths { get; set; }

    [Required]
    public string Description { get; set; } = string.Empty;

    public int InstituteId { get; set; }
}