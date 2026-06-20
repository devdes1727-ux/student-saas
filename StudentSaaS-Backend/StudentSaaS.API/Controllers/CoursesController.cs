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
    public async Task<IActionResult> Get([FromQuery] int instituteId = 1)
    {
        var courses = await _context.Courses
            .Where(c => c.InstituteId == instituteId)
            .Select(c => new
            {
                id = c.Id,
                name = c.CourseName,
                description = c.Description,
                fee = c.Fee,
                duration = c.DurationMonths + " months",
                level = c.Category,
                maxStudents = 20,
                ageGroup = "All Ages",
                isActive = c.IsActive,
                courseCode = c.CourseCode
            })
            .ToListAsync();

        return Ok(courses);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var c = await _context.Courses.FindAsync(id);

        if (c == null)
            return NotFound();

        return Ok(new
        {
            id = c.Id,
            name = c.CourseName,
            description = c.Description,
            fee = c.Fee,
            duration = c.DurationMonths,
            level = c.Category,
            maxStudents = 20,
            ageGroup = "All Ages",
            isActive = c.IsActive,
            courseCode = c.CourseCode
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCourseDto dto)
    {
        var course = new Course
        {
            CourseName = dto.CourseName,
            CourseCode = dto.CourseCode,
            Fee = dto.Fee,
            DurationMonths = dto.DurationMonths,
            Description = dto.Description,
            Category = dto.Category,
            InstituteId = dto.InstituteId == 0 ? 1 : dto.InstituteId,
            IsActive = true
        };

        _context.Courses.Add(course);
        await _context.SaveChangesAsync();

        return Ok(course);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Course course)
    {
        if (id != course.Id)
            return BadRequest();

        _context.Entry(course).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return Ok(course);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var course = await _context.Courses.FindAsync(id);

        if (course == null)
            return NotFound();

        _context.Courses.Remove(course);
        await _context.SaveChangesAsync();

        return Ok();
    }
}
