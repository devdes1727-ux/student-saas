using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentSaaS.API.Data;
using StudentSaaS.API.Models;
using StudentSaaS.API.Helpers;

namespace StudentSaaS.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class StaffController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public StaffController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/staff
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] string? role = "Teacher",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int instituteId = 1)
    {
        var query = _context.Staff.Where(s => s.InstituteId == instituteId);

        if (!string.IsNullOrWhiteSpace(role))
        {
            query = query.Where(s => s.Role == role);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(s => s.Name.Contains(search) || s.Phone.Contains(search) || s.Email.Contains(search));
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

    // GET: api/staff/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var staff = await _context.Staff.FindAsync(id);
        if (staff == null)
            return NotFound("Staff member not found");

        return Ok(staff);
    }

    // POST: api/staff
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Staff staff)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        staff.InstituteId = staff.InstituteId == 0 ? 1 : staff.InstituteId;
        staff.JoiningDate = staff.JoiningDate == default ? DateTime.UtcNow : staff.JoiningDate;

        _context.Staff.Add(staff);
        await _context.SaveChangesAsync();

        // Automatically create User login for Teacher if email is provided
        if (staff.Role.Equals("Teacher", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(staff.Email))
        {
            var userExists = await _context.Users.AnyAsync(x => x.Email == staff.Email);
            if (!userExists)
            {
                var user = new User
                {
                    Name = staff.Name,
                    Email = staff.Email,
                    PasswordHash = PasswordHelper.Hash("Password@123"), // default password
                    Role = "Teacher",
                    Phone = staff.Phone,
                    InstituteId = staff.InstituteId,
                    IsActive = true
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }
        }

        return CreatedAtAction(nameof(Get), new { id = staff.Id }, staff);
    }

    // PUT: api/staff/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Staff staff)
    {
        if (id != staff.Id)
            return BadRequest("ID mismatch");

        var existing = await _context.Staff.FindAsync(id);
        if (existing == null)
            return NotFound("Staff member not found");

        existing.Name = staff.Name;
        existing.Role = staff.Role;
        existing.Phone = staff.Phone;
        existing.Email = staff.Email;
        existing.Subject = staff.Subject;
        existing.JoiningDate = staff.JoiningDate;
        existing.ActiveStatus = staff.ActiveStatus;

        _context.Staff.Update(existing);
        await _context.SaveChangesAsync();

        return Ok(existing);
    }

    // DELETE: api/staff/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var staff = await _context.Staff.FindAsync(id);
        if (staff == null)
            return NotFound("Staff member not found");

        // Remove staff member and corresponding user account if exists
        _context.Staff.Remove(staff);
        
        if (!string.IsNullOrWhiteSpace(staff.Email))
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == staff.Email);
            if (user != null)
            {
                _context.Users.Remove(user);
            }
        }

        await _context.SaveChangesAsync();
        return Ok(new { message = "Staff member deleted successfully." });
    }
}
