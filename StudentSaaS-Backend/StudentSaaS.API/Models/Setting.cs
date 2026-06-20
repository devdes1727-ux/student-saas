namespace StudentSaaS.API.Models;

public class Setting
{
    public int Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public int InstituteId { get; set; }
}
