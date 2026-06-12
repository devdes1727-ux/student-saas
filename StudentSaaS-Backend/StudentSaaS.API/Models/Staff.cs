namespace StudentSaaS.API.Models;

public class Staff
{
    public int Id { get; set; }

    public int InstituteId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;
}