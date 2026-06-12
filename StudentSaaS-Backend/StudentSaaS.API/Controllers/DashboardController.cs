using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentSaaS.API.Data;

namespace StudentSaaS.API.Controllers;
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public DashboardController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var totalStudents = await _context.Students.CountAsync();
        var totalStaff = await _context.Staff.CountAsync();
        var totalCourses = await _context.Courses.CountAsync();
        var totalInstitutes = await _context.Institutes.CountAsync();

        return Ok(new
        {
            totalInstitutes,
            totalStudents,
            totalStaff,
            totalCourses,
            monthlyRevenue = 0
        });
    }
}