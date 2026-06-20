using System;

namespace StudentSaaS.API.Models;

public class Batch
{
    public int Id { get; set; }
    public string BatchName { get; set; } = string.Empty;
    public string StartTime { get; set; } = string.Empty; // e.g. "17:00"
    public string EndTime { get; set; } = string.Empty; // e.g. "18:00"

    public int? TeacherId { get; set; }
    public Staff? Teacher { get; set; }

    public int CourseId { get; set; }
    public Course? Course { get; set; }

    public int InstituteId { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
}
