using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentSaaS.API.Data;
using StudentSaaS.API.DTOs;
using StudentSaaS.API.Helpers;
using StudentSaaS.API.Models;

namespace StudentSaaS.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BatchesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public BatchesController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int instituteId = 1)
    {
        var role = ClaimsHelper.GetRole(User);
        var userId = ClaimsHelper.GetUserId(User);
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

        var query = _context.Batches
            .Include(b => b.Teacher)
            .Include(b => b.Course)
            .Where(b => b.InstituteId == instituteId && !b.IsDeleted);

        if (role.Equals("Teacher", StringComparison.OrdinalIgnoreCase))
        {
            var staff = await _context.Staff.FirstOrDefaultAsync(s => s.Email == email && !s.IsDeleted);
            if (staff != null)
            {
                query = query.Where(b => b.TeacherId == staff.Id);
            }
            else
            {
                // If staff not found, return empty
                query = query.Where(b => false);
            }
        }

        var list = await query.ToListAsync();
        var dtos = _mapper.Map<List<BatchDto>>(list);
        return Ok(dtos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var batch = await _context.Batches
            .Include(b => b.Teacher)
            .Include(b => b.Course)
            .FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);

        if (batch == null)
            return NotFound("Batch not found");

        var role = ClaimsHelper.GetRole(User);
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

        if (role.Equals("Teacher", StringComparison.OrdinalIgnoreCase))
        {
            var staff = await _context.Staff.FirstOrDefaultAsync(s => s.Email == email && !s.IsDeleted);
            if (staff == null || batch.TeacherId != staff.Id)
                return Forbid("Access denied to this batch");
        }

        var dto = _mapper.Map<BatchDto>(batch);
        return Ok(dto);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] BatchDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var batch = _mapper.Map<Batch>(dto);
        batch.InstituteId = 1;
        batch.CreatedAt = DateTime.UtcNow;

        _context.Batches.Add(batch);
        await _context.SaveChangesAsync();

        return Ok(_mapper.Map<BatchDto>(batch));
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] BatchDto dto)
    {
        if (id != dto.Id)
            return BadRequest("ID mismatch");

        var existing = await _context.Batches.FindAsync(id);
        if (existing == null || existing.IsDeleted)
            return NotFound("Batch not found");

        existing.BatchName = dto.BatchName;
        existing.StartTime = dto.StartTime;
        existing.EndTime = dto.EndTime;
        existing.TeacherId = dto.TeacherId;
        existing.CourseId = dto.CourseId;
        existing.IsActive = dto.IsActive;
        existing.UpdatedAt = DateTime.UtcNow;

        _context.Batches.Update(existing);
        await _context.SaveChangesAsync();

        return Ok(_mapper.Map<BatchDto>(existing));
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var batch = await _context.Batches.FindAsync(id);
        if (batch == null || batch.IsDeleted)
            return NotFound("Batch not found");

        batch.IsDeleted = true;
        batch.UpdatedAt = DateTime.UtcNow;

        _context.Batches.Update(batch);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Batch soft-deleted successfully" });
    }
}
