using System;

namespace StudentSaaS.API.Models;

public class Event
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string ImageUrl { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public DateTime EventDate { get; set; }

    public string Location { get; set; } = string.Empty;

    public int InstituteId { get; set; }

    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
}
