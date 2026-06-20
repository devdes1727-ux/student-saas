using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentSaaS.API.Data;
using StudentSaaS.API.Models;

namespace StudentSaaS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PublicController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PublicController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/public/stats
    [HttpGet("stats")]
    public async Task<IActionResult> GetStats([FromQuery] int instituteId = 1)
    {
        var totalStudents = await _context.Students.CountAsync(s => s.InstituteId == instituteId && !s.IsDeleted);
        var totalCourses = await _context.Courses.CountAsync(c => c.InstituteId == instituteId && c.IsActive && !c.IsDeleted);
        var totalTeachers = await _context.Staff.CountAsync(t => t.InstituteId == instituteId && t.Role == "Teacher" && t.ActiveStatus && !t.IsDeleted);
        
        var yearsExperience = 10;

        return Ok(new
        {
            totalStudents,
            totalCourses,
            totalTeachers,
            yearsExperience
        });
    }

    // GET: api/public/testimonials
    [HttpGet("testimonials")]
    public async Task<IActionResult> GetTestimonials([FromQuery] int instituteId = 1)
    {
        var testimonials = await _context.Testimonials
            .Where(t => t.InstituteId == instituteId && !t.IsDeleted)
            .ToListAsync();

        return Ok(testimonials);
    }

    // GET: api/public/courses
    [HttpGet("courses")]
    public async Task<IActionResult> GetCourses([FromQuery] int instituteId = 1)
    {
        var courses = await _context.Courses
            .Where(c => c.InstituteId == instituteId && c.IsActive && !c.IsDeleted)
            .ToListAsync();

        return Ok(courses);
    }

    // GET: api/public/events
    [HttpGet("events")]
    public async Task<IActionResult> GetEvents([FromQuery] int instituteId = 1)
    {
        var events = await _context.Events
            .Where(e => e.InstituteId == instituteId && !e.IsDeleted)
            .OrderBy(e => e.EventDate)
            .ToListAsync();

        return Ok(events);
    }

    // GET: api/public/gallery
    [HttpGet("gallery")]
    public async Task<IActionResult> GetGallery([FromQuery] int instituteId = 1)
    {
        var gallery = await _context.GalleryImages
            .Where(g => g.InstituteId == instituteId && !g.IsDeleted)
            .ToListAsync();

        return Ok(gallery);
    }

    // POST: api/public/contact
    [HttpPost("contact")]
    public async Task<IActionResult> SubmitContactForm([FromBody] Contact model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        model.CreatedAt = DateTime.UtcNow;
        model.InstituteId = model.InstituteId == 0 ? 1 : model.InstituteId;
        model.IsDeleted = false;

        _context.Contacts.Add(model);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Thank you for contacting us. Your message has been received." });
    }

    // POST: api/public/enquiry
    [HttpPost("enquiry")]
    public async Task<IActionResult> SubmitAdmissionEnquiry([FromBody] Enquiry model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        model.CreatedAt = DateTime.UtcNow;
        model.Status = "Pending";
        model.InstituteId = model.InstituteId == 0 ? 1 : model.InstituteId;
        model.IsDeleted = false;

        _context.Enquiries.Add(model);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Thank you! Your admission enquiry has been submitted successfully." });
    }
}
