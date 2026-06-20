using System;

namespace StudentSaaS.API.DTOs;

public class EnquiryDto
{
    public int Id { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string ParentName { get; set; } = string.Empty;
    public string ParentPhone { get; set; } = string.Empty;
    public int CourseId { get; set; }
    public string? CourseName { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; }
}
