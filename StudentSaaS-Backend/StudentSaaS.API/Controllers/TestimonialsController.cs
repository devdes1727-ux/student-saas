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
public class TestimonialsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public TestimonialsController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int instituteId = 1)
    {
        var list = await _context.Testimonials
            .Where(t => t.InstituteId == instituteId && !t.IsDeleted)
            .ToListAsync();

        var dtos = _mapper.Map<List<TestimonialDto>>(list);
        return Ok(dtos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var test = await _context.Testimonials
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

        if (test == null)
            return NotFound("Testimonial not found");

        return Ok(_mapper.Map<TestimonialDto>(test));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TestimonialDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var test = _mapper.Map<Testimonial>(dto);
        test.InstituteId = 1;
        test.CreatedAt = DateTime.UtcNow;

        _context.Testimonials.Add(test);
        await _context.SaveChangesAsync();

        return Ok(_mapper.Map<TestimonialDto>(test));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] TestimonialDto dto)
    {
        if (id != dto.Id)
            return BadRequest("ID mismatch");

        var existing = await _context.Testimonials.FindAsync(id);
        if (existing == null || existing.IsDeleted)
            return NotFound("Testimonial not found");

        existing.Name = dto.Name;
        existing.Role = dto.Role;
        existing.Message = dto.Message;
        existing.Rating = dto.Rating;
        existing.ImageUrl = dto.ImageUrl ?? string.Empty;
        existing.UpdatedAt = DateTime.UtcNow;

        _context.Testimonials.Update(existing);
        await _context.SaveChangesAsync();

        return Ok(_mapper.Map<TestimonialDto>(existing));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var test = await _context.Testimonials.FindAsync(id);
        if (test == null || test.IsDeleted)
            return NotFound("Testimonial not found");

        test.IsDeleted = true;
        test.UpdatedAt = DateTime.UtcNow;

        _context.Testimonials.Update(test);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Testimonial soft-deleted successfully" });
    }
}
