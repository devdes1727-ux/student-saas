namespace StudentSaaS.API.Models;

public class Institute
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string LogoUrl { get; set; } = string.Empty;

    public string ThemeSettings { get; set; } = "light";

    public string EmailSettings { get; set; } = string.Empty;

    public string WhatsAppNumber { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public string SocialMediaLinks { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}