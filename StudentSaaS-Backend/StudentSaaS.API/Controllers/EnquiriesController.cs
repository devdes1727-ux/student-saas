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
public class EnquiriesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public EnquiriesController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int instituteId = 1)
    {
        var list = await _context.Enquiries
            .Include(e => e.Course)
            .Where(e => e.InstituteId == instituteId && !e.IsDeleted)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();

        var dtos = _mapper.Map<List<EnquiryDto>>(list);
        return Ok(dtos);
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
    {
        var enquiry = await _context.Enquiries.FindAsync(id);
        if (enquiry == null || enquiry.IsDeleted)
            return NotFound("Enquiry not found");

        if (status != "Pending" && status != "Approved" && status != "Rejected")
            return BadRequest("Invalid status value");

        enquiry.Status = status;
        _context.Enquiries.Update(enquiry);
        await _context.SaveChangesAsync();

        return Ok(new { message = $"Enquiry status updated to {status}", status });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var enquiry = await _context.Enquiries.FindAsync(id);
        if (enquiry == null || enquiry.IsDeleted)
            return NotFound("Enquiry not found");

        enquiry.IsDeleted = true;
        _context.Enquiries.Update(enquiry);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Enquiry removed successfully" });
    }
}
