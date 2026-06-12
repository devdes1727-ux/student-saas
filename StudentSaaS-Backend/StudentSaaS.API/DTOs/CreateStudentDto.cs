using System.ComponentModel.DataAnnotations;

namespace StudentSaaS.API.DTOs;

public class CreateStudentDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Phone { get; set; } = string.Empty;

    [Required]
    public string ParentName { get; set; } = string.Empty;

    [Required]
    public string ParentPhone { get; set; } = string.Empty;

    [Required]
    public string Course { get; set; } = string.Empty;

    [Required]
    public string Batch { get; set; } = string.Empty;
}