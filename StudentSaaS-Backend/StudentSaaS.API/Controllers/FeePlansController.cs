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
using StudentSaaS.API.Models;

namespace StudentSaaS.API.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class FeePlansController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public FeePlansController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int instituteId = 1)
    {
        var list = await _context.FeePlans
            .Include(f => f.Course)
            .Where(f => f.InstituteId == instituteId && !f.IsDeleted)
            .ToListAsync();

        var dtos = _mapper.Map<List<FeePlanDto>>(list);
        return Ok(dtos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var plan = await _context.FeePlans
            .Include(f => f.Course)
            .FirstOrDefaultAsync(f => f.Id == id && !f.IsDeleted);

        if (plan == null)
            return NotFound("Fee Plan not found");

        return Ok(_mapper.Map<FeePlanDto>(plan));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] FeePlanDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var plan = _mapper.Map<FeePlan>(dto);
        plan.InstituteId = 1;
        plan.CreatedAt = DateTime.UtcNow;

        _context.FeePlans.Add(plan);
        await _context.SaveChangesAsync();

        return Ok(_mapper.Map<FeePlanDto>(plan));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] FeePlanDto dto)
    {
        if (id != dto.Id)
            return BadRequest("ID mismatch");

        var existing = await _context.FeePlans.FindAsync(id);
        if (existing == null || existing.IsDeleted)
            return NotFound("Fee Plan not found");

        existing.PlanName = dto.PlanName;
        existing.CourseId = dto.CourseId;
        existing.TotalAmount = dto.TotalAmount;
        existing.Installments = dto.Installments;
        existing.Description = dto.Description;
        existing.IsActive = dto.IsActive;
        existing.UpdatedAt = DateTime.UtcNow;

        _context.FeePlans.Update(existing);
        await _context.SaveChangesAsync();

        return Ok(_mapper.Map<FeePlanDto>(existing));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var plan = await _context.FeePlans.FindAsync(id);
        if (plan == null || plan.IsDeleted)
            return NotFound("Fee Plan not found");

        plan.IsDeleted = true;
        plan.UpdatedAt = DateTime.UtcNow;

        _context.FeePlans.Update(plan);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Fee Plan soft-deleted successfully" });
    }
}
