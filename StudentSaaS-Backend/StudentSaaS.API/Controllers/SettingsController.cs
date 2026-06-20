using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentSaaS.API.Data;
using StudentSaaS.API.Models;

namespace StudentSaaS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SettingsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public SettingsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/settings
    [HttpGet]
    public async Task<IActionResult> GetSettings([FromQuery] int instituteId = 1)
    {
        var institute = await _context.Institutes.FindAsync(instituteId);
        if (institute == null)
        {
            // Seed a default one if not found
            institute = new Institute
            {
                Id = instituteId,
                Name = "Sivaram Kalaikoodam Art Academy",
                Phone = "+91 98765 43210",
                IsActive = true
            };
            _context.Institutes.Add(institute);
            await _context.SaveChangesAsync();
        }

        return Ok(institute);
    }

    // PUT: api/settings (Admin only)
    [Authorize(Roles = "Admin")]
    [HttpPut]
    public async Task<IActionResult> UpdateSettings([FromBody] Institute model, [FromQuery] int instituteId = 1)
    {
        var institute = await _context.Institutes.FindAsync(instituteId);
        if (institute == null)
        {
            return NotFound("Institute not found");
        }

        institute.Name = model.Name;
        institute.Phone = model.Phone;
        institute.LogoUrl = model.LogoUrl ?? string.Empty;
        institute.ThemeSettings = model.ThemeSettings ?? "light";
        institute.EmailSettings = model.EmailSettings ?? string.Empty;
        institute.WhatsAppNumber = model.WhatsAppNumber ?? string.Empty;
        institute.Address = model.Address ?? string.Empty;
        institute.SocialMediaLinks = model.SocialMediaLinks ?? string.Empty;

        _context.Institutes.Update(institute);
        await _context.SaveChangesAsync();

        return Ok(institute);
    }
}
