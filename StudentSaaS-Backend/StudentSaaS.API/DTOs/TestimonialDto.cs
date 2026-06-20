namespace StudentSaaS.API.DTOs;

public class TestimonialDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public int InstituteId { get; set; }
}
