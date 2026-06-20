using System;

namespace StudentSaaS.API.Models;

public class FeePlan
{
    public int Id { get; set; }
    public string PlanName { get; set; } = string.Empty;
    public int CourseId { get; set; }
    public Course? Course { get; set; }
    public decimal TotalAmount { get; set; }
    public int Installments { get; set; } = 1;
    public string Description { get; set; } = string.Empty;

    public int InstituteId { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
}
