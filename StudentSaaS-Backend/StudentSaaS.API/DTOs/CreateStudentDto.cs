using System;
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

    public int Age { get; set; }

    public string Gender { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public DateTime? AdmissionDate { get; set; }

    public string Status { get; set; } = "Active";

    public string ProfileImage { get; set; } = string.Empty;
}