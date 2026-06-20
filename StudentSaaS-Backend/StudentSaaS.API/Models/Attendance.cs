using System;

namespace StudentSaaS.API.Models;

public class Attendance
{
    public int Id { get; set; }

    public int StudentId { get; set; }

    public string StudentName { get; set; } = string.Empty;

    public DateTime Date { get; set; }

    public string Status { get; set; } = "Present"; // Present, Absent, Leave, Late

    public string Remarks { get; set; } = string.Empty;

    public int InstituteId { get; set; }

    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
}
