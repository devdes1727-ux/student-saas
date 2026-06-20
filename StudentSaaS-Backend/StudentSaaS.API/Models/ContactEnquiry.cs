using System;

namespace StudentSaaS.API.Models;

public class ContactEnquiry
{
    public int Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public string Source { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int InstituteId { get; set; }
}
