using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentSaaS.API.Data;
using StudentSaaS.API.DTOs;
using StudentSaaS.API.Models;
using StudentSaaS.API.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace StudentSaaS.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public StudentsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] string? course,
        [FromQuery] string? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int instituteId = 1)
    {
        var role = ClaimsHelper.GetRole(User);
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

        var query = _context.Students
            .Where(s => s.InstituteId == instituteId && !s.IsDeleted);

        if (role.Equals("Teacher", StringComparison.OrdinalIgnoreCase))
        {
            var staff = await _context.Staff.FirstOrDefaultAsync(s => s.Email == email && !s.IsDeleted);
            if (staff != null)
            {
                var assignedBatches = await _context.Batches
                    .Where(b => b.TeacherId == staff.Id && !b.IsDeleted && b.IsActive)
                    .Select(b => b.BatchName)
                    .ToListAsync();
                query = query.Where(s => assignedBatches.Contains(s.Batch));
            }
            else
            {
                query = query.Where(s => false);
            }
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(s => s.Name.Contains(search) || s.Phone.Contains(search) || s.ParentName.Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(course))
        {
            query = query.Where(s => s.Course == course);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(s => s.Status == status);
        }

        var total = await query.CountAsync();
        var items = await query
            .OrderBy(s => s.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(new
        {
            total,
            page,
            pageSize,
            items
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var student = await _context.Students.FindAsync(id);

        if (student == null || student.IsDeleted)
            return NotFound("Student not found");

        var role = ClaimsHelper.GetRole(User);
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

        if (role.Equals("Teacher", StringComparison.OrdinalIgnoreCase))
        {
            var staff = await _context.Staff.FirstOrDefaultAsync(s => s.Email == email && !s.IsDeleted);
            if (staff == null)
                return Forbid("Access denied");

            var assignedBatches = await _context.Batches
                .Where(b => b.TeacherId == staff.Id && !b.IsDeleted && b.IsActive)
                .Select(b => b.BatchName)
                .ToListAsync();

            if (!assignedBatches.Contains(student.Batch))
                return Forbid("Access denied to this student");
        }

        return Ok(student);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateStudentDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var student = new Student
        {
            Name = dto.Name,
            Phone = dto.Phone,
            ParentName = dto.ParentName,
            ParentPhone = dto.ParentPhone,
            Course = dto.Course,
            Batch = dto.Batch,
            Age = dto.Age,
            Gender = dto.Gender ?? "",
            Address = dto.Address ?? "",
            Email = dto.Email ?? "",
            AdmissionDate = dto.AdmissionDate ?? DateTime.UtcNow,
            Status = dto.Status ?? "Active",
            ProfileImage = dto.ProfileImage ?? "",
            InstituteId = 1 // Default
        };

        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        // Auto-create User account if email is provided and doesn't exist
        if (!string.IsNullOrWhiteSpace(student.Email))
        {
            var userExists = await _context.Users.AnyAsync(x => x.Email == student.Email);
            if (!userExists)
            {
                var user = new User
                {
                    Name = student.Name,
                    Email = student.Email,
                    PasswordHash = PasswordHelper.Hash("Password@123"), // Default password
                    Role = "Student",
                    Phone = student.Phone,
                    InstituteId = 1,
                    IsActive = true
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }
        }

        return Ok(student);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Student student)
    {
        if (id != student.Id)
            return BadRequest("ID mismatch");

        var existing = await _context.Students.FindAsync(id);
        if (existing == null || existing.IsDeleted)
            return NotFound("Student not found");

        existing.Name = student.Name;
        existing.Phone = student.Phone;
        existing.ParentName = student.ParentName;
        existing.ParentPhone = student.ParentPhone;
        existing.Course = student.Course;
        existing.Batch = student.Batch;
        existing.Age = student.Age;
        existing.Gender = student.Gender;
        existing.Address = student.Address;
        existing.Email = student.Email;
        existing.AdmissionDate = student.AdmissionDate;
        existing.Status = student.Status;
        existing.ProfileImage = student.ProfileImage;

        _context.Students.Update(existing);
        await _context.SaveChangesAsync();

        return Ok(existing);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var student = await _context.Students.FindAsync(id);

        if (student == null || student.IsDeleted)
            return NotFound("Student not found");

        // Soft Delete
        student.IsDeleted = true;
        _context.Students.Update(student);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Student deleted successfully (soft delete)." });
    }
}