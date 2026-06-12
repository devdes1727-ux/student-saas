namespace StudentSaaS.API.Models;

public class Student
{
    public int Id { get; set; }

    public int InstituteId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;
    public string ParentName { get; set; } = string.Empty;

    public string ParentPhone { get; set; } = string.Empty;

    public string Course { get; set; } = string.Empty;

    public string Batch { get; set; } = string.Empty;
}