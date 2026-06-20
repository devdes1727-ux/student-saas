namespace StudentSaaS.API.DTOs;

public class BatchDto
{
    public int Id { get; set; }
    public string BatchName { get; set; } = string.Empty;
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public int? TeacherId { get; set; }
    public string? TeacherName { get; set; }
    public int CourseId { get; set; }
    public string? CourseName { get; set; }
    public bool IsActive { get; set; }
}
