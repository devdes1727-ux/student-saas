using System;

namespace StudentSaaS.API.Models;

public class Enquiry
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
    public Course? Course { get; set; }

    public string Message { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int InstituteId { get; set; }
    public bool IsDeleted { get; set; } = false;
}
