namespace StudentSaaS.API.Models;

using System;

public class Student
{
    public int Id { get; set; }

    public int InstituteId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string ParentName { get; set; } = string.Empty;

    public string ParentPhone { get; set; } = string.Empty;

    public string Course { get; set; } = string.Empty;

    public string Batch { get; set; } = string.Empty;

    public int? CourseId { get; set; }
    public Course? CourseRef { get; set; }

    public int? BatchId { get; set; }
    public Batch? BatchRef { get; set; }

    public int Age { get; set; }

    public string Gender { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public DateTime AdmissionDate { get; set; } = DateTime.UtcNow;

    public string Status { get; set; } = "Active";

    public string ProfileImage { get; set; } = string.Empty;

    public bool IsDeleted { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
}