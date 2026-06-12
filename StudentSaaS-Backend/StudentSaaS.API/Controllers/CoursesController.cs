using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentSaaS.API.Data;
using StudentSaaS.API.DTOs;
using StudentSaaS.API.Models;

namespace StudentSaaS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CoursesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CoursesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return Ok(await _context.Courses.ToListAsync());
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCourseDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (string.IsNullOrEmpty(dto.CourseName) ||
            string.IsNullOrEmpty(dto.CourseCode))
        {
            return BadRequest("Invalid course data");
        }

        var course = new Course
        {
            CourseName = dto.CourseName,
            CourseCode = dto.CourseCode,
            Fee = dto.Fee,
            DurationMonths = dto.DurationMonths,
            Description = dto.Description,
            InstituteId = dto.InstituteId,
            IsActive = true
        };

        _context.Courses.Add(course);
        await _context.SaveChangesAsync();

        return Ok(course);
    }
}