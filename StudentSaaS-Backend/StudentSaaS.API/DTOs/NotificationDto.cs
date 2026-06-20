using System;

namespace StudentSaaS.API.DTOs;

public class NotificationDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = "Info"; // Info, Success, Warning, Danger
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}
