namespace StudentSaaS.API.DTOs;

public class FeePlanDto
{
    public int Id { get; set; }
    public string PlanName { get; set; } = string.Empty;
    public int CourseId { get; set; }
    public string? CourseName { get; set; }
    public decimal TotalAmount { get; set; }
    public int Installments { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
